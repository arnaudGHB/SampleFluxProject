using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using CBS.TransactionManagement.DailyStatisticBoard.Queries;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Command;

namespace CBS.TransactionManagement.DailyStatisticBoard.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific GeneralDailyDashboard based on its unique identifier.
    /// </summary>
    public class GeneralDailyDashboardByBranchQueryHandler : IRequestHandler<GeneralDailyDashboardByBranchQuery, ServiceResponse<GeneralDailyDashboardDto>>
    {
        private readonly IGeneralDailyDashboardRepository _GeneralDailyDashboardRepository; // Repository for accessing GeneralDailyDashboard data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GeneralDailyDashboardByBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the GeneralDailyDashboardByBranchQueryHandler.
        /// </summary>
        /// <param name="GeneralDailyDashboardRepository">Repository for GeneralDailyDashboard data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GeneralDailyDashboardByBranchQueryHandler(
            IGeneralDailyDashboardRepository GeneralDailyDashboardRepository,
            IMapper mapper,
            ILogger<GeneralDailyDashboardByBranchQueryHandler> logger,
            IMediator mediator)
        {
            _GeneralDailyDashboardRepository = GeneralDailyDashboardRepository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GeneralDailyDashboardByBranchQuery to retrieve a specific GeneralDailyDashboard.
        /// </summary>
        /// <param name="request">The GeneralDailyDashboardByBranchQuery containing GeneralDailyDashboard ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GeneralDailyDashboardDto>> Handle(GeneralDailyDashboardByBranchQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            // Retrieve branch details for the given Branch ID in the request
            var branch = await GetBranch(request.BranchId);

            try
            {
                // Attempt to retrieve the GeneralDailyDashboard entity from the repository based on branch and date range.
                // It checks if there's any dashboard data for the branch between the specified 'DateFrom' and 'DateTo'.
                var entity = await _GeneralDailyDashboardRepository
                    .FindBy(x => x.BranchId == request.BranchId && x.Date.Date >= request.DateFrom.Date && x.Date.Date <= request.DateTo.Date)
                    .FirstOrDefaultAsync();

                if (entity != null)
                {
                    // If data is found, map the GeneralDailyDashboard entity to the GeneralDailyDashboardDto DTO.
                    var GeneralDailyDashboardDto = _mapper.Map<GeneralDailyDashboardDto>(entity);

                    // Return a successful response with the DTO data.
                    return ServiceResponse<GeneralDailyDashboardDto>.ReturnResultWith200(GeneralDailyDashboardDto);
                }
                else
                {
                    // Set an error message if no dashboard data is found for the branch and date range.
                    errorMessage = $"No daily dashboard data was found for branch '{branch.name}' within the specified date-range. [{request.DateFrom.Date}-{request.DateTo.Date}]";

                    // Log the error with the error message.
                    _logger.LogError(errorMessage);

                    // Log and audit the error asynchronously, tagging it as a 404 Not Found with warning level.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GeneralDailyDashboard, LogLevelInfo.Warning);
                    entity = new Data.Entity.DailyStatisticBoard.GeneralDailyDashboard();
                    var GeneralDailyDashboardDto = _mapper.Map<GeneralDailyDashboardDto>(entity);
                    GeneralDailyDashboardDto.BranchCode = branch.branchCode;
                    GeneralDailyDashboardDto.BranchName = branch.name;
                    // Return a 404 Not Found response, indicating that data was not found.
                    return ServiceResponse<GeneralDailyDashboardDto>.ReturnResultWith200(GeneralDailyDashboardDto);
                }
            }
            catch (Exception e)
            {
                // Set the error message in case of an exception during processing.
                errorMessage = $"An error occurred while retrieving the daily general dashboard for branch '{branch.name}' within the specified date-range. [{request.DateFrom.Date}-{request.DateTo.Date}]. Error details: {e.Message}";

                // Log the error with the full error message.
                _logger.LogError(errorMessage);

                // Log and audit the error asynchronously, tagging it as a 500 Internal Server Error with error level.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GeneralDailyDashboard, LogLevelInfo.Error);

                // Return a 500 Internal Server Error response with the error message.
                return ServiceResponse<GeneralDailyDashboardDto>.Return500(errorMessage);
            }
        }
        private async Task<BranchDto> GetBranch(string BranchId)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = BranchId }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
    }

}
