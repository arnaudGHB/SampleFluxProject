using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountClass based on its unique identifier.
    /// </summary>
    public class GetChartOfAccountManagementPositionQueryHandler : IRequestHandler<GetChartOfAccountManagementPositionQuery, ServiceResponse<ChartOfAccountManagementPositionDto>>
    {
        private readonly IChartOfAccountManagementPositionRepository _ChartOfAccountManagementPositionRepository;
            private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetChartOfAccountManagementPositionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetChartOfAccountManagementPositionQueryHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetChartOfAccountManagementPositionQueryHandler(
            IChartOfAccountManagementPositionRepository AccountClassRepository,
               IAccountCategoryRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<GetChartOfAccountManagementPositionQueryHandler> logger)
        {
            _ChartOfAccountManagementPositionRepository = AccountClassRepository;
           
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetChartOfAccountManagementPositionQueryHandler to retrieve a specific AccountClass.
        /// </summary>
        /// <param name="request">The GetChartOfAccountManagementPositionQueryHandler containing AccountClass ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChartOfAccountManagementPositionDto>> Handle(GetChartOfAccountManagementPositionQuery request, CancellationToken cancellationToken)
        {
            string accountNumber = "";
            string errorMessage = "";
            try
            {

                var model = await _ChartOfAccountManagementPositionRepository.FindAsync( request.Id);// && c.IsDeleted == false).ToList();

                if (model != null)
                {
                    if (model.IsDeleted)
                    {
                        string message = "ChartOfAccountManagementPosition has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<ChartOfAccountManagementPositionDto>.Return404(message);
                    }
                    else
                    {
                        // Map the ChartOfAccount entity to ChartOfAccountDto and return it with a success response
                        var AccountDto = _mapper.Map<ChartOfAccountManagementPositionDto>(model);
                        return ServiceResponse<ChartOfAccountManagementPositionDto>.ReturnResultWith200(AccountDto);
                    }


                }
                else
                {
                    // If the ChartOfAccount entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("ChartOfAccountManagementPosition not found.");
                    return ServiceResponse<ChartOfAccountManagementPositionDto>.Return404();
                }
              }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountClass: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<ChartOfAccountManagementPositionDto>.Return500(e);
            }
        }
    }
}