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
using CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Commands;
using CBS.TransactionManagement.Repository.VaultP;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Handlers
{
    /// <summary>
    /// Handles the command to add a new CashReplenishment.
    /// </summary>
    public class AddCashReplenishmentPrimaryTellerHandler : IRequestHandler<AddCashReplenishmentPrimaryTellerCommand, ServiceResponse<CashReplenishmentPrimaryTellerDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;

        private readonly ITellerRepository _tellerRepository; // Repository for accessing CashReplenishment data.
        private readonly ICashReplenishmentPrimaryTellerRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCashReplenishmentPrimaryTellerHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IVaultRepository _vaultRepository; // Repository for accessing CashReplenishment data.

        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddRemittanceHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCashReplenishmentPrimaryTellerHandler(
            ICashReplenishmentPrimaryTellerRepository CashReplenishmentRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddCashReplenishmentPrimaryTellerHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            ITellerRepository tellerRepository = null,
            IAccountRepository accountRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            IVaultRepository vaultRepository = null)
        {
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _tellerRepository = tellerRepository;
            _accountRepository = accountRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _vaultRepository=vaultRepository;
        }

        /// <summary>
        /// Handles the AddCashReplenishmentCommand to add a new CashReplenishment.
        /// </summary>
        /// <param name="request">The AddCashReplenishmentCommand containing CashReplenishment data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<CashReplenishmentPrimaryTellerDto>> Handle(AddCashReplenishmentPrimaryTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the active teller for the day
                var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();

                // Retrieve the teller and their associated account
                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Check if a CashReplenishment exists for the teller and branch
                var existingCashReplenishment = await _CashReplenishmentRepository.FindBy(c => c.TellerId == teller.Id &&
                                                                                                    c.ApprovedStatus != Status.Approved.ToString() &&
                                                                                                    !c.IsDeleted &&
                                                                                                    c.BranchId == teller.BranchId)
                                                                                     .FirstOrDefaultAsync();

                _vaultRepository.VerifySufficientFunds(request.RequestedAmount);
                // If a CashReplenishment already exists, return a conflict response
                if (existingCashReplenishment != null)
                {
                    var errorMessage = $"You are having a pending request that needs to be validated.";
                    LogAndAuditError(errorMessage, request, 409);
                    return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return409(errorMessage);
                }
                // Check if the requested amount exceeds the teller's maximum amount to manage
                if ((tellerAccount.Balance + request.RequestedAmount) > teller.MaximumAmountToManage)
                {
                    string errorMessage = $"The amount you requested has exceeded your teller's maximum amount.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(errorMessage, request, 403);
                    return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return403(errorMessage);
                }
                // Map the command to a CashReplenishmentPrimaryTeller entity
                var cashReplenishmentEntity = _mapper.Map<CashReplenishmentPrimaryTeller>(request);
                string Reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"CSR-PT{_userInfoToken.BranchCode}-");
                cashReplenishmentEntity.Id = BaseUtilities.GenerateUniqueNumber();
                cashReplenishmentEntity.ApprovedStatus = Status.Pending.ToString();
                cashReplenishmentEntity.BranchId = tellerAccount.BranchId;
                cashReplenishmentEntity.TellerId = teller.Id;
                cashReplenishmentEntity.ApprovedDate = DateTime.MaxValue;
                cashReplenishmentEntity.InitializeDate = BaseUtilities.UtcNowToDoualaTime();
                cashReplenishmentEntity.RequesterUserId = _userInfoToken.Id;
                cashReplenishmentEntity.RequesterName = _userInfoToken.FullName;
                cashReplenishmentEntity.TransactionReference = Reference;
                // Add the new CashReplenishment entity to the repository
                _CashReplenishmentRepository.Add(cashReplenishmentEntity);
                // Save changes to the database
                await _uow.SaveAsync();

                // Log and audit the successful creation of CashReplenishment
                string msg = $"CashReplenishment for primary-teller {teller.Name} was created successfully";
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the CashReplenishment entity to CashReplenishmentDto and return it with a success response
                var cashReplenishmentDto = _mapper.Map<CashReplenishmentPrimaryTellerDto>(cashReplenishmentEntity);
                return ServiceResponse<CashReplenishmentPrimaryTellerDto>.ReturnResultWith200(cashReplenishmentDto, msg);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CashReplenishment: {e.Message}";
                LogAndAuditError(errorMessage, request, 500, e);
                return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return500(e, errorMessage);
            }
        }

        // Utility method to log and audit errors
        private void LogAndAuditError(string errorMessage, object request, int statusCode, Exception exception = null)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }



    }

}
