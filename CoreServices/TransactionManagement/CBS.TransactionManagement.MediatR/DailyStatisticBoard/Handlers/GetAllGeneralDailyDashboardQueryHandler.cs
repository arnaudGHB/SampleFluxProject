using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.DailyStatisticBoard.Queries;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;

namespace CBS.TransactionManagement.DailyStatisticBoard.Handlers
{
    /// <summary>
    /// Handles the retrieval of all GeneralDailyDashboard based on the GetAllGeneralDailyDashboardQuery.
    /// </summary>
    public class GetAllGeneralDailyDashboardQueryHandler : IRequestHandler<GetAllGeneralDailyDashboardQuery, ServiceResponse<List<GeneralDailyDashboardDto>>>
    {
        private readonly IGeneralDailyDashboardRepository _GeneralDailyDashboardRepository; // Repository for accessing GeneralDailyDashboards data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllGeneralDailyDashboardQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllGeneralDailyDashboardQueryHandler.
        /// </summary>
        /// <param name="GeneralDailyDashboardRepository">Repository for GeneralDailyDashboards data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllGeneralDailyDashboardQueryHandler(
            IGeneralDailyDashboardRepository GeneralDailyDashboardRepository,
            IMapper mapper, ILogger<GetAllGeneralDailyDashboardQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _GeneralDailyDashboardRepository = GeneralDailyDashboardRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllGeneralDailyDashboardQuery to retrieve all GeneralDailyDashboards.
        /// </summary>
        /// <param name="request">The GetAllGeneralDailyDashboardQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GeneralDailyDashboardDto>>> Handle(GetAllGeneralDailyDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Attempt to retrieve all GeneralDailyDashboard entities within the specified date range.
                var entities = await _GeneralDailyDashboardRepository.All
                    .Where(x => x.Date.Date >= request.DateFrom.Date && x.Date.Date <= request.DateTo.Date)
                    .ToListAsync();

                // Check if any data was found for the given date range.
                if (entities == null || !entities.Any())
                {
                    // Set a warning message if no data was found for the specified period.
                    var notFoundMessage = $"No GeneralDailyDashboard data was found for all branches within the specified date-range. [{request.DateFrom.Date}-{request.DateTo.Date}]";
                    _logger.LogWarning(notFoundMessage);

                    // Log and audit the "not found" scenario with warning level.
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GeneralDailyDashboard, LogLevelInfo.Warning);

                    // Return a 404 Not Found response with the warning message.
                    return ServiceResponse<List<GeneralDailyDashboardDto>>.Return404(notFoundMessage);
                }

                // Map the retrieved entities to a list of GeneralDailyDashboardDto objects.
                var dashboardDtoList = _mapper.Map<List<GeneralDailyDashboardDto>>(entities);

                // Return a successful response with the list of DTOs.
                return ServiceResponse<List<GeneralDailyDashboardDto>>.ReturnResultWith200(dashboardDtoList);
            }
            catch (Exception e)
            {
                // Set an error message for logging and response.
                var errorMessage = $"No daily dashboard data was found for all branches within the specified date-range. [{request.DateFrom.Date}-{request.DateTo.Date}]";

                // Log the error with full details, including the exception message.
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GeneralDailyDashboard, LogLevelInfo.Error);

                // Return a 500 Internal Server Error response, indicating a failure to retrieve data.
                return ServiceResponse<List<GeneralDailyDashboardDto>>.Return500(errorMessage);
            }
        }

    }
}
