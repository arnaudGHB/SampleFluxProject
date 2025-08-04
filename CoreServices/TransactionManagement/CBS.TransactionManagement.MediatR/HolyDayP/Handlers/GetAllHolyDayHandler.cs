using AutoMapper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayP.Queries;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Queries;
using CBS.TransactionManagement.Repository.HolyDayP;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllHolyDayHandler : IRequestHandler<GetAllHolyDayQuery, ServiceResponse<List<HolyDayDto>>>
    {
        private readonly IHolyDayRepository _HolyDayRepository; // Repository for accessing HolyDays data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllHolyDayHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the GetAllHolyDayQueryHandler.
        /// </summary>
        /// <param name="HolyDayRepository">Repository for HolyDays data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllHolyDayHandler(
            IHolyDayRepository HolyDayRepository,
            IMapper mapper, ILogger<GetAllHolyDayHandler> logger, IMediator mediator = null)
        {
            // Assign provided dependencies to local variables.
            _HolyDayRepository = HolyDayRepository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetAllHolyDayQuery to retrieve all HolyDays.
        /// </summary>
        /// <param name="request">The GetAllHolyDayQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<HolyDayDto>>> Handle(GetAllHolyDayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Query to fetch active (non-deleted) HolyDayRecurring records
                var query = _HolyDayRepository.All.AsNoTracking().Where(x => !x.IsDeleted);

                // Apply branch filter if requested
                if (request.ByBranch)
                {
                    if (string.IsNullOrEmpty(request.BranchId))
                    {
                        // Return bad request if BranchId is required but not provided
                        return ServiceResponse<List<HolyDayDto>>.Return400("BranchId is required when ByBranch is true.");
                    }

                    // Filter holidays by the specific BranchId
                    query = query.Where(x => x.BranchId == request.BranchId);
                }

                // Fetch filtered HolyDayRecurring entities
                var entities = await query.ToListAsync(cancellationToken);

                // Fetch branch data: a single branch if BranchId is specified, otherwise all branches
                var branches = request.ByBranch && !string.IsNullOrEmpty(request.BranchId)
                    ? new List<BranchDto> { await GetBranchInfo(request.BranchId) }.Where(b => b != null).ToList()
                    : await GetBranchesAsync();

                // Map entities to DTOs
                var result = _mapper.Map<List<HolyDayDto>>(entities);

                // Attach branch information to the DTOs
                result.ForEach(dto =>
                {
                    dto.Branch = branches.FirstOrDefault(branch => branch.id == dto.BranchId);
                });

                // Return success response with mapped data
                return ServiceResponse<List<HolyDayDto>>.ReturnResultWith200(result);
            }
            catch (Exception e)
            {
                // Log the error and return an internal server error response
                _logger.LogError($"Failed to get HolyDayRecurring data: {e.Message}");
                return ServiceResponse<List<HolyDayDto>>.Return500(e, "Failed to get HolyDayRecurring data.");
            }
        }
        private async Task<BranchDto> GetBranchInfo(string branchId)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchId };
            var branchResponse = await _mediator.Send(branchCommandQuery);
            // Check if retrieving branch information was successful
            if (branchResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting branch information.");
            return branchResponse.Data;
        }
        private async Task<List<BranchDto>> GetBranchesAsync()
        {
            var branchCommandQuery = new GetBranchesCommand { };
            var branchResponse = await _mediator.Send(branchCommandQuery);
            // Check if retrieving branch information was successful
            if (branchResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting branch information.");
            return branchResponse.Data;
        }
    }
}
