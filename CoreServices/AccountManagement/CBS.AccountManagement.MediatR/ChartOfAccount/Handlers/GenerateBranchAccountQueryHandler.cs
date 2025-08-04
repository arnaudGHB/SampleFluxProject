using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.ChartOfAccount.MediatR.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.ChartOfAccount.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all ChartOfAccounts based on the GetAllChartOfAccountQuery.
    /// </summary>
    public class GenerateBranchAccountQueryHandler : IRequestHandler<GenerateBranchAccountQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IChartOfAccountRepository _chartOfAccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GenerateBranchAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly  IAccountRepository _accountRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly   IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;


        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GenerateBranchAccountQueryHandler(
            IChartOfAccountRepository chartOfAccountRepository,
            IMapper mapper, ILogger<GenerateBranchAccountQueryHandler> logger, IAccountRepository? accountRepository, UserInfoToken? userInfoToken, IUnitOfWork<POSContext>? uow, IMediator? mediator, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
            _mapper = mapper;
            _logger = logger;
            _accountRepository = accountRepository;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetAllChartOfAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllChartOfAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountDto>>> Handle(GenerateBranchAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all Accounts entities from the repository
                var entities = await _chartOfAccountRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                foreach (var chartOfAccount in entities) 
                {
                    var AccountManagementPosition = GetAccountNumberManagementPosition(chartOfAccount);

                    var model = new  AddAccountCommand
                    {
                        AccountNumberManagementPosition = AccountManagementPosition,
                        AccountOwnerId = _userInfoToken.BranchId,
                        AccountName = chartOfAccount.LabelEn + " " + _userInfoToken.BranchName,
                        AccountNumber = chartOfAccount.AccountNumber,
                        AccountNumberNetwok = (chartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + _userInfoToken.BranchCode).PadRight(12, '0')+ AccountManagementPosition,
                        AccountNumberCU = (chartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode).PadRight(9, '0')+ AccountManagementPosition,
                        ChartOfAccountManagementPositionId = chartOfAccount.Id,
                        AccountTypeId = _userInfoToken.BranchId,
                         AccountCategoryId = chartOfAccount.AccountCartegoryId
                    };
                    await _mediator.Send(model);
                }
                    return ServiceResponse<List<AccountDto>>.ReturnResultWith200(_mapper.Map<List<AccountDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }

        public string GetAccountNumberManagementPosition(AccountManagement.Data.ChartOfAccount chartOfAccount)
        {
            string numberString = string.Empty;
            if (chartOfAccount.HasManagementPostion.Value)
            {
                var models = _chartOfAccountManagementPositionRepository.FindBy(f => f.ChartOfAccountId == chartOfAccount.Id);

                if (models.Any())
                {
                    numberString = models.FirstOrDefault().PositionNumber.PadRight(3, '0');
                }
            }
            return numberString;
        }

    }
}