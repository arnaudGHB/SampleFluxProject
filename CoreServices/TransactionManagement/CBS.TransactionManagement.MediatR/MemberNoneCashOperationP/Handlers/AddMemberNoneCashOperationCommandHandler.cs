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
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.Data.Entity.MemberNoneCashOperationP;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands;
using CBS.TransactionManagement.Repository.MemberNoneCashOperationP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Member None Cash Operation.
    /// </summary>
    public class AddMemberNoneCashOperationCommandHandler : IRequestHandler<AddMemberNoneCashOperationCommand, ServiceResponse<MemberNoneCashOperationDto>>
    {
        private readonly IAccountRepository _accountRepository; // Repository for account-related data.
        private readonly IMemberNoneCashOperationRepository _noneCashOperationRepository; // Repository for None Cash Operations.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services.
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accounting day information.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for handling transactions.
        private readonly IMapper _mapper; // AutoMapper for mapping objects.
        private readonly ILogger<AddMemberNoneCashOperationCommandHandler> _logger; // Logger for capturing logs.
        private readonly UserInfoToken _userInfoToken; // Token holding the current user's information.

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMemberNoneCashOperationCommandHandler"/> class.
        /// </summary>
        public AddMemberNoneCashOperationCommandHandler(
            IAccountRepository accountRepository,
            IMemberNoneCashOperationRepository noneCashOperationRepository,
            IUnitOfWork<TransactionContext> uow,
            IMapper mapper,
            ILogger<AddMemberNoneCashOperationCommandHandler> logger,
            UserInfoToken userInfoToken,
            IAPIUtilityServicesRepository utilityServicesRepository,
            IAccountingDayRepository accountingDayRepository)
        {
            _accountRepository = accountRepository;
            _noneCashOperationRepository = noneCashOperationRepository;
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _utilityServicesRepository = utilityServicesRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the command to add a new Member None Cash Operation.
        /// </summary>
        /// <param name="request">The command containing details of the None Cash Operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MemberNoneCashOperationDto>> Handle(AddMemberNoneCashOperationCommand request, CancellationToken cancellationToken)
        {
            // Generate a unique reference number for tracking the transaction.
            string referenceNumber = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Retrieve the current accounting day for the user's branch.
                var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Validate the requested amount is positive.
                if (request.Amount <= 0)
                {
                    string error = "Invalid amount entered. Amount must be greater than zero.";
                    _logger.LogWarning(error);
                    await BaseUtilities.LogAndAuditAsync(error, request, HttpStatusCodeEnum.Forbidden, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Warning, referenceNumber);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return403(error);
                }

                // Retrieve the account using the provided account number.
                var account = await _accountRepository.GetAccountByAccountNumber(request.AccountNUmber);
                if (account == null)
                {
                    string error = $"Account with number {request.AccountNUmber} not found.";
                    _logger.LogWarning(error);
                    await BaseUtilities.LogAndAuditAsync(error, request, HttpStatusCodeEnum.NotFound, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Warning, referenceNumber);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return404(error);
                }

                // Validate interbranch operation
                if (account.BranchId != _userInfoToken.BranchID)
                {
                    string error = $"Interbranch operations are not allowed. The account belongs to Branch Code {account.BranchCode}, while the request is made from Branch {_userInfoToken.BranchName}.";
                    _logger.LogWarning(error);
                    await BaseUtilities.LogAndAuditAsync(error, request, HttpStatusCodeEnum.Forbidden, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Warning, referenceNumber);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return403(error);
                }

                // Retrieve the transfer parameters associated with the account's product.
                var transferParameter = account.Product.TransferParameters.FirstOrDefault(x => x.TransferType == "Local");
                if (transferParameter == null)
                {
                    string error = "No local transfer parameter found for the specified account.";
                    _logger.LogWarning(error);
                    await BaseUtilities.LogAndAuditAsync(error, request, HttpStatusCodeEnum.BadRequest, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Warning, referenceNumber);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return400(error);
                }

                // Retrieve the customer details using the provided member reference.
                var customer = await _utilityServicesRepository.GetCustomer(request.MemberReference);
                if (customer == null)
                {
                    string error = $"Customer with reference {request.MemberReference} not found.";
                    _logger.LogWarning(error);
                    await BaseUtilities.LogAndAuditAsync(error, request, HttpStatusCodeEnum.NotFound, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Warning, referenceNumber);
                    return ServiceResponse<MemberNoneCashOperationDto>.Return404(error);
                }
                if (request.BookingDirection!=OperationType.Credit.ToString())
                {
                    // Check if the account has sufficient balance for the requested transfer and charges.
                    _accountRepository.CheckBalanceForTransferCharges(request.Amount, account, transferParameter, request.Amount, customer.LegalForm);
                }


               
                // Map the request to the None Cash Operation entity using AutoMapper.
                var noneCashOperationEntity = _mapper.Map<MemberNoneCashOperation>(request);
                if (request.BookingDirection==OperationType.Debit.ToString())
                {
                    noneCashOperationEntity.Source="MemberAccount";
                    
                }
                else
                {
                    noneCashOperationEntity.Source="NotMemberAccount";
                }
                string defult = "N/A";
                noneCashOperationEntity.Id = BaseUtilities.GenerateUniqueNumber();
                noneCashOperationEntity.BranchId = _userInfoToken.BranchID;
                noneCashOperationEntity.BranchCode = _userInfoToken.BranchCode;
                noneCashOperationEntity.BranchName = _userInfoToken.BranchName;
                noneCashOperationEntity.CreatedDate = BaseUtilities.UtcNowToDoualaTime();
                noneCashOperationEntity.AccountingDate = accountingDay;
                noneCashOperationEntity.TransactionReference = referenceNumber;
                noneCashOperationEntity.InitiatedByUSerId = _userInfoToken.Id;
                noneCashOperationEntity.InitiatedUserName = _userInfoToken.FullName;
                noneCashOperationEntity.ApprovalStatus = Status.Pending.ToString();
                noneCashOperationEntity.RequestDate = BaseUtilities.UtcNowToDoualaTime();
                noneCashOperationEntity.AccountId = account.Id;
                // Set approval-related fields for the initial state.
                noneCashOperationEntity.ApprovedByUSerId = defult;
                noneCashOperationEntity.ApprovedBy = defult;
                noneCashOperationEntity.ApprovalDate = DateTime.MaxValue;
                noneCashOperationEntity.ApprovalComment = defult;

                // Add the None Cash Operation entity to the repository.
                _noneCashOperationRepository.Add(noneCashOperationEntity);
                await _uow.SaveAsync();

                // Log and audit the success of the operation.
                string successMessage = "Member none-cash operation recorded successfully.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Information, referenceNumber);

                // Map the entity to a DTO and return it in the response.
                var operationDto = _mapper.Map<MemberNoneCashOperationDto>(noneCashOperationEntity);
                return ServiceResponse<MemberNoneCashOperationDto>.ReturnResultWith200(operationDto, successMessage);
            }
            catch (InvalidOperationException ex)
            {
                // Handle validation errors.
                string message = $"Validation error: {ex.Message}";
                _logger.LogError(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.BadRequest, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Error, referenceNumber);
                return ServiceResponse<MemberNoneCashOperationDto>.Return400(message);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors.
                string error = $"An unexpected error occurred: {ex.Message}";
                _logger.LogError(error);
                await BaseUtilities.LogAndAuditAsync(error, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberNoneCashOperationRequest, LogLevelInfo.Error, referenceNumber);
                return ServiceResponse<MemberNoneCashOperationDto>.Return500(ex, error);
            }
        }
    }

}
