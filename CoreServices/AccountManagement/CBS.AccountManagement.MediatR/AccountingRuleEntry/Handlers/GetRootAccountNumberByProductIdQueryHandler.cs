using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetRootAccountNumberByProductIdQueryHandler : IRequestHandler<GetRootAccountNumberByProductIdQuery, ServiceResponse<AccountMap>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRootAccountNumberByProductIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="_AccountingRuleEntryRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetRootAccountNumberByProductIdQueryHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            IMapper mapper, ILogger<GetRootAccountNumberByProductIdQueryHandler> logger, IChartOfAccountRepository chartOfAccountRepository, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _chartOfAccountRepository = chartOfAccountRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            // Assign provided dependencies to local variables.
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountMap>> Handle(GetRootAccountNumberByProductIdQuery request, CancellationToken cancellationToken)
        {
            AccountMap  model = new AccountMap(); 
            try
            {
                // Retrieve all Accounts entities from the repository 
                var entities = await _AccountingRuleEntryRepository.All.Where(c=>c.IsDeleted==false && c.ProductAccountingBookId==request.GetProductKey()).ToListAsync();
                if (entities.Any())
                {
                    var objet= entities.FirstOrDefault();
                    var ACCcHARTXX = await _chartOfAccountManagementPositionRepository.FindAsync(objet.DeterminationAccountId);
                    if (ACCcHARTXX != null) 
                    {
                        var ACCcHART = await _chartOfAccountRepository.FindAsync(ACCcHARTXX.ChartOfAccountId);
                        if (ACCcHART != null)
                        {
                            model.AccountNumber = ACCcHART.AccountNumber.PadRight(6, '0');
                            model.AccountName = ACCcHART.LabelEn;
                            return ServiceResponse<AccountMap>.ReturnResultWith200(model);
                        }
                        else
                        {
                            var message = string.Empty;
                            message = $"No chart of account has been set with this productId:{request.GetProductKey()}. Please contact System Admin";
                            return ServiceResponse<AccountMap>.Return403(message);
                        }

                    }
                    else
                    {
                        var message = string.Empty;
                        message = $"No chart of account Management Position has been set with this DeterminationAccountId:{objet.DeterminationAccountId}. Please contact System Admin";
                        return ServiceResponse<AccountMap>.Return403(message);
                    }
                   
                }
                else {
                    var message = string.Empty;
                    message = "No fee entry has been configured for the now. Please contact System Admin";
                    return ServiceResponse<AccountMap>.Return403(message);
                }
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<AccountMap>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}