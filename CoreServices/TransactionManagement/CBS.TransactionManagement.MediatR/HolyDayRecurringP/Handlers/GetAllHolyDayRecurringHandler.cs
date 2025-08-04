using AutoMapper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Data.Entity.HolyDayRecurringP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Handlers;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Queries;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringMediaR.HolyDayRecurringP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllHolyDayRecurringHandler : IRequestHandler<GetAllHolyDayRecurringQuery, ServiceResponse<List<HolyDayRecurringDto>>>
    {
        private readonly IHolyDayRecurringRepository _HolyDayRecurringRepository; // Repository for accessing HolyDayRecurrings data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllHolyDayRecurringHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMediator _mediator;
        public GetAllHolyDayRecurringHandler(
            IHolyDayRecurringRepository HolyDayRecurringRepository,
            IMapper mapper,
            ILogger<GetAllHolyDayRecurringHandler> logger,
            IMediator mediator = null)
        {
            _HolyDayRecurringRepository = HolyDayRecurringRepository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<ServiceResponse<List<HolyDayRecurringDto>>> Handle(GetAllHolyDayRecurringQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Query to fetch active (non-deleted) HolyDayRecurring records
                var query = _HolyDayRecurringRepository.All.AsNoTracking().Where(x => !x.IsDeleted);

                // If ByBranch is true, we need to include both centralized data and filtered data by branch
                if (request.ByBranch)
                {
                    if (string.IsNullOrEmpty(request.BranchId))
                    {
                        // Return bad request if BranchId is required but not provided
                        return ServiceResponse<List<HolyDayRecurringDto>>.Return400("BranchId is required when ByBranch is true.");
                    }

                    // Fetch data for the specific branch
                    query = query.Where(x => x.BranchId == request.BranchId);

                    // Fetch centralized data (i.e., records where BranchId is null or represents centralized data)
                    var centralizedData = await _HolyDayRecurringRepository.All.AsNoTracking()
                        .Where(x => !x.IsDeleted && x.BranchId == null) // Assuming centralized data has BranchId == null
                        .ToListAsync(cancellationToken);

                    // Fetch filtered HolyDayRecurring data for the requested branch
                    var branchData = await query.ToListAsync(cancellationToken);

                    // Combine both the centralized data and branch-specific data
                    var combinedData = centralizedData.Concat(branchData).ToList();

                    // Map entities to DTOs
                    var result = _mapper.Map<List<HolyDayRecurringDto>>(combinedData);

                    // Fetch branch data: a single branch if BranchId is specified, otherwise all branches
                    List<BranchDto> branches = await GetBranchesAsync();

                    // Attach branch information to the DTOs
                    result.ForEach(dto =>
                    {
                        dto.Branch = branches.FirstOrDefault(branch => branch.id == dto.BranchId);
                    });

                    // Return success response with mapped data
                    return ServiceResponse<List<HolyDayRecurringDto>>.ReturnResultWith200(result);
                }
                else
                {
                    // If ByBranch is false, fetch all data (including centralized data) for all branches
                    var entities = await query.ToListAsync(cancellationToken);

                    // Fetch branch data: all branches
                    List<BranchDto> branches = await GetBranchesAsync();

                    // Map entities to DTOs
                    var result = _mapper.Map<List<HolyDayRecurringDto>>(entities);

                    // Attach branch information to the DTOs
                    result.ForEach(dto =>
                    {
                        dto.Branch = branches.FirstOrDefault(branch => branch.id == dto.BranchId);
                    });

                    // Return success response with mapped data
                    return ServiceResponse<List<HolyDayRecurringDto>>.ReturnResultWith200(result);
                }
            }
            catch (Exception e)
            {
                // Log the error and return an internal server error response
                _logger.LogError($"Failed to get HolyDayRecurring data: {e.Message}");
                return ServiceResponse<List<HolyDayRecurringDto>>.Return500(e, "Failed to get HolyDayRecurring data.");
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
            var branchCommandQuery = new GetBranchesCommand {};
            var branchResponse = await _mediator.Send(branchCommandQuery);
            // Check if retrieving branch information was successful
            if (branchResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting branch information.");
            return branchResponse.Data;
        }
    }
}
