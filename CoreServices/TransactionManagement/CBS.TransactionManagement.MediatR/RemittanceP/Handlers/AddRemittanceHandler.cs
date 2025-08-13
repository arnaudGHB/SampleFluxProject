using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using System.Globalization;
using AutoMapper.Internal;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Repository.RemittanceP;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{
    /// <summary>
    /// Handles the command to add a new remittance.
    /// </summary>
    public class AddRemittanceHandler : IRequestHandler<AddRemittanceCommand, ServiceResponse<RemittanceDto>>
    {
        // Dependencies injected into the handler
        private readonly IAccountRepository _accountRepository; // Repository to manage account-related operations
        private readonly IRemittanceRepository _remittanceRepository; // Repository for remittance data management
        private readonly IMapper _mapper; // AutoMapper for object mapping
        private readonly ILogger<AddRemittanceHandler> _logger; // Logger for logging information and errors
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for transaction management
        private readonly UserInfoToken _userInfoToken; // User information token for logged-in user context
        public IMediator _mediator { get; set; } // Mediator for sending commands and events
        private readonly ISavingProductRepository _savingProductRepository; // Repository to manage account-related operations
        private readonly IDailyTransactionCodeGenerator _dailyTransactionCodeGenerator;
        /// <summary>
        /// Constructor for initializing the AddRemittanceHandler.
        /// </summary>
        public AddRemittanceHandler(
            IRemittanceRepository remittanceRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddRemittanceHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IAccountRepository accountRepository = null,
            ISavingProductRepository savingProductRepository = null,
            IDailyTransactionCodeGenerator dailyTransactionCodeGenerator = null)
        {
            _remittanceRepository = remittanceRepository; // Initialize remittance repository
            _mapper = mapper; // Initialize mapper
            _userInfoToken = userInfoToken; // Initialize user info token
            _logger = logger; // Initialize logger
            _uow = uow; // Initialize unit of work
            _mediator = mediator; // Initialize _mediator
            _accountRepository = accountRepository; // Initialize account repository
            _savingProductRepository=savingProductRepository;
            _dailyTransactionCodeGenerator=dailyTransactionCodeGenerator;
        }

        /// <summary>
        /// Handles the AddRemittanceCommand to add a new remittance.
        /// </summary>
        /// <param name="request">The AddRemittanceCommand containing remittance data.</param>
        /// <param name="cancellationToken">A cancellation token for async operations.</param>
        public async Task<ServiceResponse<RemittanceDto>> Handle(AddRemittanceCommand request, CancellationToken cancellationToken)
        {
            string correlationId = Guid.NewGuid().ToString(); // Unique tracking ID

            try
            {
                _logger.LogInformation($"[INFO] Processing new remittance request. CorrelationId: '{correlationId}', Account: '{request.AccountNumber}', Amount: '{BaseUtilities.FormatCurrency(request.InitailAmount)}'.");

                // 🔍 Step 1: Validate and Retrieve Account Details
                var remittanceAccount = await _accountRepository.GetRemittanceAccount(request.AccountNumber, request.RemittanceType, OperationType.Transfer.ToString());
                if (remittanceAccount == null)
                {
                    string errorMessage = $"[ERROR] Remittance account details could not be found for Account '{request.AccountNumber}'. CorrelationId: '{correlationId}'.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.Remittance, LogLevelInfo.Error, correlationId);
                    return ServiceResponse<RemittanceDto>.Return400(errorMessage);
                }

                // 🔍 Step 2: Generate a Unique Transaction Reference
                string reference = await _dailyTransactionCodeGenerator.ReserveTransactionCode(
                    _userInfoToken.BranchCode, OperationPrefix.Remittance, TransactionType.TRANSFER, isInterBranch: false);

                request.ExternalReference = string.IsNullOrWhiteSpace(request.ExternalReference) || request.ExternalReference == "N/A"
                    ? reference
                    : request.ExternalReference;

                reference = request.ExternalReference;

                // 🔍 Step 3: Check for Duplicate Transactions
                var existingRemittance = await _remittanceRepository.FindBy(r => r.TransactionReference == reference && !r.IsDeleted).ToListAsync();
                if (existingRemittance.Any())
                {
                    string duplicateMessage = $"[WARNING] The External Reference '{reference}' already exists. Please provide a unique reference. CorrelationId: '{correlationId}'.";
                    _logger.LogWarning(duplicateMessage);
                    await BaseUtilities.LogAndAuditAsync(duplicateMessage, request, HttpStatusCodeEnum.Conflict, LogAction.Remittance, LogLevelInfo.Warning, correlationId);
                    return ServiceResponse<RemittanceDto>.Return409(duplicateMessage);
                }

                // 🔍 Step 4: Map Request to Entity
                var remittance = _mapper.Map<Remittance>(request);
                remittance.Id = BaseUtilities.GenerateUniqueNumber();
                remittance.Status = Status.Pending.ToString();
                remittance.TransactionReference = reference;
                remittance.InitiatedBy = _userInfoToken.FullName;
                remittance.InitiationDate = BaseUtilities.UtcNowToDoualaTime();
                remittance.AccountId = remittanceAccount.Id;
                remittance.ExternalReference = reference;

                // 🔍 Step 5: Validate Transfer Type
                if (request.TransferType == TransferType.Incoming_International.ToString())
                {
                    if (!request.InternationalTransfterDate.HasValue)
                    {
                        string missingDateError = $"[ERROR] International Transfer Date is required for incoming international transfers. CorrelationId: '{correlationId}'.";
                        _logger.LogError(missingDateError);
                        return ServiceResponse<RemittanceDto>.Return400(missingDateError);
                    }
                    remittance.InternationalTransfterDate = request.InternationalTransfterDate.Value;
                    remittance.InitiationDate = request.InternationalTransfterDate.Value;
                }

                // 🔍 Step 6: Calculate Receiver Amount Based on Charge Type
                remittance.IsChargesInclussive = request.ChargeType == ChargeType.Inclussive.ToString();
                remittance.ReceiverAmount = remittance.IsChargesInclussive ? request.InitailAmount - request.Fee : request.InitailAmount;
                remittance.TotalAmount = remittance.IsChargesInclussive ? request.InitailAmount : request.InitailAmount + request.Fee;


                // 🔍 Step 7: Validate Transfer Configuration
                var remittanceProduct = await _savingProductRepository
                    .FindBy(x => x.Id == remittanceAccount.ProductId)
                    .Include(x => x.TransferParameters)
                    .FirstOrDefaultAsync();

                if (remittanceProduct == null || remittanceProduct.TransferParameters == null || !remittanceProduct.TransferParameters.Any())
                {
                    string configError = $"[ERROR] No valid transfer parameters found for this remittance product. CorrelationId: '{correlationId}'.";
                    _logger.LogError(configError);
                    return ServiceResponse<RemittanceDto>.Return400(configError);
                }

                // 🔍 Step 8: Determine Transfer Configuration
                var transferConfig = remittanceProduct.TransferParameters.FirstOrDefault(tp => tp.TransferType == request.TransferType);
                if (transferConfig == null)
                {
                    string configMissing = $"[ERROR] No valid transfer configuration found for {request.TransferType} transfers. CorrelationId: '{correlationId}'.";
                    _logger.LogError(configMissing);
                    return ServiceResponse<RemittanceDto>.Return400(configMissing);
                }

                // 🔍 Step 9: Calculate Commission Based on Transfer Parameter
                decimal totalFee = request.Fee;
                decimal sourceBranchShare = Math.Round(totalFee * (transferConfig.SourceBrachOfficeShare / 100M), 2);
                decimal destinationBranchShare = Math.Round(totalFee * (transferConfig.DestinationBranchOfficeShare / 100M), 2);
                decimal headOfficeShare = Math.Round(totalFee * (transferConfig.HeadOfficeShare / 100M), 2);

                remittance.SourceBranchCommision = sourceBranchShare;
                remittance.RecivingBranchCommision = destinationBranchShare;
                remittance.HeadOfficeCommision = headOfficeShare;
                remittance.RecevingBranchTotalAmount =remittance.ReceiverAmount+remittance.RecivingBranchCommision;
                // 🔍 Step 10: Save the Remittance Transaction
                _remittanceRepository.Add(remittance);
                await _uow.SaveAsync();
                await _dailyTransactionCodeGenerator.MarkTransactionAsSuccessful(request.ExternalReference);

                // 🔍 Step 11: Log and Audit the Successful Transaction
                string successMessage = $"[SUCCESS] The remittance request has been successfully created. Pending approval. Reference Number: '{reference}'.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Remittance, LogLevelInfo.Information, correlationId);

                // 🔍 Step 12: Return the Response
                var remittanceDto = _mapper.Map<RemittanceDto>(remittance);
                return ServiceResponse<RemittanceDto>.ReturnResultWith200(remittanceDto, successMessage);
            }
            catch (Exception ex)
            {
                // 🔄 Rollback the transaction reference if an error occurs
                await _dailyTransactionCodeGenerator.RevertTransactionCode(request.ExternalReference);

                // 🔍 Log error details
                string errorMessage = $"[ERROR] An unexpected error occurred while processing the remittance request. CorrelationId: '{correlationId}'. Details: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Remittance, LogLevelInfo.Error, correlationId);

                return ServiceResponse<RemittanceDto>.Return500(ex, errorMessage);
            }
        }



        //public async Task<ServiceResponse<RemittanceDto>> Handle(AddRemittanceCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {

        //        // Retrieve remittance account details based on account number and remittance type
        //        var remittanceAccount = await _accountRepository.GetRemittanceAccount(
        //            request.AccountNumber,
        //            request.RemittanceType,
        //            OperationType.Transfer.ToString());

        //        // Map the command data to a Remittance entity
        //        var remittance = _mapper.Map<Remittance>(request);

        //        // Generate a unique reference for the transaction if none is provided
        //        string reference = await DailyTransactionCodeGenerator.ReserveTransactionCode(_userInfoToken.BranchCode, OperationPrefix.Remittance, OperationType.Transfer, isInterBranch:false);

        //        if (request.ExternalReference == "N/A")
        //        {
        //            request.ExternalReference = reference;
        //        }
        //        else
        //        {
        //            reference = request.ExternalReference;
        //        }
        //        // Check if the ExternalReference already exists
        //        var existingRemittance = await _remittanceRepository.FindBy(r => r.TransactionReference == request.ExternalReference && r.IsDeleted==false).ToListAsync();
        //        if (existingRemittance.Any())
        //        {
        //            string duplicateReferenceMessage = $"The External Reference '{request.ExternalReference}' already exists. Please provide a unique reference.";
        //            await BaseUtilities.LogAndAuditAsync(
        //                                                duplicateReferenceMessage,
        //                                                request,
        //                                                HttpStatusCodeEnum.Conflict,
        //                                                LogAction.Remittance,
        //                                                LogLevelInfo.Warning);
        //            _logger.LogInformation(duplicateReferenceMessage);

        //            return ServiceResponse<RemittanceDto>.Return409(duplicateReferenceMessage);
        //        }


        //        // Set additional remittance properties
        //        remittance.Id = BaseUtilities.GenerateUniqueNumber(); // Unique ID for the remittance
        //        remittance.Status = Status.Pending.ToString(); // Set status to pending
        //        remittance.TransactionReference = reference; // Assign transaction reference
        //        remittance.InitiatedBy = _userInfoToken.FullName; // Assign the initiator's name
        //        remittance.InitiationDate = BaseUtilities.UtcNowToDoualaTime(); // Set initiation date
        //        remittance.AccountId = remittanceAccount.Id;
        //        remittance.ExternalReference = reference;
        //        if (request.TransferType==TransferType.Incoming_International.ToString())
        //        {
        //            remittance.InternationalTransfterDate=request.InternationalTransfterDate;
        //            remittance.InitiationDate = request.InternationalTransfterDate.Value; // Set initiation date
        //        }
        //        if (request.ChargeType==ChargeType.Inclussive.ToString())
        //        {
        //            remittance.IsChargesInclussive=true;
        //            remittance.ReceiverAmount=request.InitailAmount-request.Fee;
        //        }
        //        else
        //        {
        //            remittance.ReceiverAmount=request.InitailAmount;
        //        }
        //        var remittanceproduct = await _savingProductRepository.FindBy(x => x.Id==remittanceAccount.ProductId).Include(x => x.TransferParameters).FirstOrDefaultAsync();
        //        if (remittanceproduct==null)
        //        {
        //           // throw exception to user
        //        }
        //        if (remittanceproduct.TransferParameters==null or 0)
        //        {
        //            // throw exception to user
        //        }

        //        var transfterConfig= //Get the transfter where its type is local
        //        // Add the remittance entity to the repository
        //        _remittanceRepository.Add(remittance);

        //        // Save changes to the database
        //        await _uow.SaveAsync();

        //        // Log and audit the successful creation of the remittance
        //        string successMessage = "The remittance request has been successfully created. It is now pending approval for further processing. You will be notified once the request has been reviewed.";
        //        await BaseUtilities.LogAndAuditAsync(
        //            successMessage,
        //            request,
        //            HttpStatusCodeEnum.OK,
        //            LogAction.Remittance,
        //            LogLevelInfo.Information);

        //        // Map the saved entity to a DTO and return success response
        //        var remittanceDto = _mapper.Map<RemittanceDto>(remittance);
        //        return ServiceResponse<RemittanceDto>.ReturnResultWith200(remittanceDto, successMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error details and return a 500 Internal Server Error response
        //        string errorMessage = $"An unexpected error occurred while processing the remittance request. Details: {ex.Message}. Please contact the system administrator for assistance.";
        //        await BaseUtilities.LogAndAuditAsync(
        //            errorMessage,
        //            request,
        //            HttpStatusCodeEnum.InternalServerError,
        //            LogAction.Remittance,
        //            LogLevelInfo.Error);

        //        return ServiceResponse<RemittanceDto>.Return500(ex, errorMessage);
        //    }
        //}




    }
}
