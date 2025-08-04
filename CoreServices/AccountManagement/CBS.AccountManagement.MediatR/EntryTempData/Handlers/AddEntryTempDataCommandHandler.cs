using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountFeature.
    /// </summary>
    public class AddEntryTempDataCommandHandler : IRequestHandler<AddEntryTempDataCommand, ServiceResponse<EntryTempDataDto>>
    {
        private readonly IEntryTempDataRepository _entryTempDataRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddEntryTempDataCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfaccountRepository;
        /// <summary>
        /// Constructor for initializing the AddAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddEntryTempDataCommandHandler(
            IEntryTempDataRepository AccountFeatureRepository,
            IAccountingEntriesServices accountingEntriesServices,
            IMapper mapper,
            ILogger<AddEntryTempDataCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IAccountRepository? accountRepository,
            IAccountingRuleEntryRepository? accountingRuleEntryRepository,
            IChartOfAccountManagementPositionRepository? chartOfaccountRepository)
        {
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _entryTempDataRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _accountRepository = accountRepository;
            _chartOfaccountRepository = chartOfaccountRepository;
            _accountingEntriesServices = accountingEntriesServices;
        }

        /// <summary>
        /// Handles the AddAccountFeatureCommand to add a new AccountFeature.
        /// </summary>
        /// <param name="request">The AddAccountFeatureCommand containing AccountFeature data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EntryTempDataDto>> Handle(AddEntryTempDataCommand request, CancellationToken cancellationToken)
        { string errorMessage = string.Empty;
            try
            {


                var accountModel = await _accountRepository.FindAsync(request.AccountId);
                //if ((await _accountingEntriesServices.ChackIfOperationIsValid(GetOperationType(request.BookingDirection) == AccountOperationType.CREDIT, accountModel, Convert.ToDecimal(request.Amount), GetOperationType(request.BookingDirection))) == false)
                //{
                //    throw new ArgumentException($"The account balance of {accountModel.AccountNumberCU}-{accountModel.AccountName} is {accountModel.CurrentBalance}XFA does not permit you to processes this transaction");
                //}

                var existingAccountEntry=  _entryTempDataRepository.FindBy(c => c.AccountId== request.AccountId&&c.Reference == request.Reference&&c.CreatedBy==_userInfoToken.Id);

                //if (existingAccountEntry.Any())
                //{
                //    var models = existingAccountEntry.FirstOrDefault();
                //      errorMessage = $"There is already an Accounting EntryTempData with name {request.AccountName}. ";
                   
                //    _logger.LogError(errorMessage);
                //    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AddEntryTempDataCommand, LogLevelInfo.Error);
                //    throw new ArgumentException(errorMessage);
                //    //return ServiceResponse<EntryTempDataDto>.Return409(errorMessage);
                //}
                var IsInUsed =  await CheckIfAccountIsOperationsAccount(request.AccountId);
                if (IsInUsed.Item1)
                {
                    
                     errorMessage = IsInUsed.Item2;

                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,request,HttpStatusCodeEnum.Unauthorized,LogAction.AddEntryTempDataCommand,LogLevelInfo.Warning);

                    return ServiceResponse<EntryTempDataDto>.Return409(errorMessage);
                }
                var AccountEntry = _mapper.Map<EntryTempData>(request);

                AccountEntry.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "ETD");
                AccountEntry.AccountingEventId = "MANUAL USER";
                // Add the new AccountFeature entity to the repository
                _entryTempDataRepository.Add(AccountEntry);
        
                // Map the AccountFeature entity to AccountFeatureDto and return it with a success response
                var model = _mapper.Map<EntryTempDataDto>(AccountEntry);

                errorMessage = $"Accounting with name {request.AccountName} with amount {request.Amount} was successfully added.";
                await _uow.SaveAsyncWithOutAffectingBranchId();
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.AddEntryTempDataCommand, LogLevelInfo.Information);
                return ServiceResponse<EntryTempDataDto>.ReturnResultWith200(model);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                 errorMessage = $"Error occurred while saving AccountFeature: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AddEntryTempDataCommand, LogLevelInfo.Error);
                throw e;
            }
        }

       
        private async Task<(bool isOperationsAccount, string message)> CheckIfAccountIsOperationsAccount(string accountId)
        {
            var account = await _accountRepository.FindAsync(accountId);
            if (account == null)
                return (true, "The account does not exist");

            var chartOfAccount = await _chartOfaccountRepository.FindAsync(account.ChartOfAccountManagementPositionId);
            if (chartOfAccount == null)
                return (true, "The MFI chartofaccount does not exist");

            var accountingRules = _accountingRuleEntryRepository
                .FindBy(x => x.DeterminationAccountId.Equals(account.ChartOfAccountManagementPositionId))
                .FirstOrDefault();

            if (accountingRules == null)
                return (false, string.Empty);

            string accountDetails = $"The account:{account.AccountNumberCU}-{account.AccountName}";

            // Check if account is used by virtual teller operations
            var virtualTellerCodes = new[] { "Virtual_Teller_MTN", "Virtual_Teller_Orange", "Virtual_Teller_Momo_cash_Collection" };
            if (virtualTellerCodes.Contains(accountingRules.EventCode))
                return (true, $"{accountDetails} is in used by operations system");

            // Check if account starts with "3""571"
            if (account.AccountNumber.StartsWith("3")|| account.AccountNumber.StartsWith("571"))
                return (true, $"{accountDetails} is in used by operations system");

            return (false, string.Empty);
        }
       
    }
}