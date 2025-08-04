using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountByEventCodeQueryHandler : IRequestHandler<GetAccountByEventCodeQuery, ServiceResponse<List<InfoAccount>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountByEventCodeQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly IMediator _mediator;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAccountByEventCodeQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountByEventCodeQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetAccountByEventCodeQueryHandler> logger,
            IAccountingEntriesServices? accountingEntriesServices,
            IMediator? mediator,
            UserInfoToken? userInfoToken)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            _accountingEntriesServices = accountingEntriesServices;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<InfoAccount>>> Handle(GetAccountByEventCodeQuery request, CancellationToken cancellationToken)
        {
            List<InfoAccount> infoAccounts = new List<InfoAccount>();
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository

                var getAccountingRuleEntryQuery = new GetAccountingRuleEntryByEventCodeQuery { EventCode = request.EventCode };
                var result = await _mediator.Send(getAccountingRuleEntryQuery);
                if (result.StatusCode.Equals(200)) 
                {
                   
                        infoAccounts = await GetParticipantAccountAsync(result.Data, request.ToBranchId, request.ToBranchCode);
                    
                 
                }
                else
                {
                    throw new Exception($"EventCode:{request.EventCode} do not exist in this system at the moment");
                }
                return ServiceResponse<List<InfoAccount>>.ReturnResultWith200(infoAccounts);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<InfoAccount>>.Return500(e);
            }
        }

        private async Task<List<InfoAccount>> GetParticipantAccountAsync(AccountingRuleEntryDto data, string toBranchId, string toBranchCode)
        {
            if (data.EventCode.ToLower().Contains("_liaison"))
            {
                var toAccount = await _accountingEntriesServices.GetAccountUsingMFIChartForliason(data.BalancingAccountId, toBranchId, toBranchCode);

                var fromAccount = await _accountingEntriesServices.GetAccountUsingMFIChart(data.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                return new List<InfoAccount>
              {
                  new InfoAccount { Id= toAccount.Id, AccountName = toAccount.AccountName, AccountNumber= toAccount.AccountNumberCU ,Type="Destination", CurrentBalance=toAccount.CurrentBalance},
                    new InfoAccount { Id= fromAccount.Id, AccountName = fromAccount.AccountName, AccountNumber= fromAccount.AccountNumberCU ,Type="Source",CurrentBalance=fromAccount.CurrentBalance},
                 };
            }

            else if (data.EventCode.ToLower().Contains("liaison_"))
            {
                var toAccount = await _accountingEntriesServices.GetAccountUsingMFIChart(data.BalancingAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode); 
                var fromAccount = await _accountingEntriesServices.GetAccountUsingMFIChartForliason(data.DeterminationAccountId, toBranchId, toBranchCode);

                return new List<InfoAccount>
              {
                  new InfoAccount { Id= toAccount.Id, AccountName = toAccount.AccountName, AccountNumber= toAccount.AccountNumberCU ,Type="Destination", CurrentBalance=toAccount.CurrentBalance},
                    new InfoAccount { Id= fromAccount.Id, AccountName = fromAccount.AccountName, AccountNumber= fromAccount.AccountNumberCU ,Type="Source",CurrentBalance=fromAccount.CurrentBalance},
                 };
            }
            else
            {
                var toAccount = await _accountingEntriesServices.GetAccountUsingMFIChart(data.BalancingAccountId, toBranchId, toBranchCode);

                var fromAccount = await _accountingEntriesServices.GetAccountUsingMFIChart(data.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                return new List<InfoAccount>
              {
                  new InfoAccount { Id= toAccount.Id, AccountName = toAccount.AccountName, AccountNumber= toAccount.AccountNumberCU ,Type="Destination", CurrentBalance=toAccount.CurrentBalance},
                    new InfoAccount { Id= fromAccount.Id, AccountName = fromAccount.AccountName, AccountNumber= fromAccount.AccountNumberCU ,Type="Source",CurrentBalance=fromAccount.CurrentBalance},
                 };
            }
        }

        
    }
}