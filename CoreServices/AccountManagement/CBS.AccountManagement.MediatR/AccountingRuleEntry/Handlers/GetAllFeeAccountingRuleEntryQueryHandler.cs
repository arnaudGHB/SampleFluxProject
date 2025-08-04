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
    public class GetOtherOperationServicesAccountingRuleEntryQueryHandler : IRequestHandler<OperationServicesAccountingRuleEntryQuery, ServiceResponse<List<RuleEntryDto>>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOtherOperationServicesAccountingRuleEntryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="_AccountingRuleEntryRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOtherOperationServicesAccountingRuleEntryQueryHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            IMapper mapper, ILogger<GetOtherOperationServicesAccountingRuleEntryQueryHandler> logger)
        {
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
        public async Task<ServiceResponse<List<RuleEntryDto>>> Handle(OperationServicesAccountingRuleEntryQuery request, CancellationToken cancellationToken)
        {
            List < RuleEntryDto > ruleEntryDtos= new List<RuleEntryDto> (); 
            try
            {
       
                if (request.OpertionType.Contains("FEE"))
                {
                    request.OpertionType = request.OpertionType.Replace("FEE", "INCOME");
                }
                // Retrieve all Accounts entities from the repository
                var entities = await _AccountingRuleEntryRepository.All.Where(c=>c.IsDeleted==false && c.EventCode.StartsWith(request.OpertionType)).ToListAsync();
                if (entities.Any())
                {
                    ruleEntryDtos = (from entity in entities
                                    select new RuleEntryDto
                                    {
                                        EventCode = entity.EventCode,
                                        AccountingRuleEntryName = entity.AccountingRuleEntryName
                                    }).ToList();
                    return ServiceResponse<List<RuleEntryDto>>.ReturnResultWith200(ruleEntryDtos);

                }
                else {
                    var message = string.Empty;
                    message = "No fee entry has been configured for the now. Please contact System Admin";
                    return ServiceResponse<List<RuleEntryDto>>.Return403(message);
                }
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<List<RuleEntryDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}