using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Branchs based on the GetAllBranchQuery.
    /// </summary>
    public class GetAllBranchQueryHandler : IRequestHandler<GetAllBranchQuery, ServiceResponse<List<BranchDto>>>
    {
        private readonly IBranchRepository _BranchRepository; // Repository for accessing Branchs data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBranchQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllBranchQueryHandler.
        /// </summary>
        /// <param name="BranchRepository">Repository for Branchs data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBranchQueryHandler(
            IBranchRepository BranchRepository,
            IMapper mapper, ILogger<GetAllBranchQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _BranchRepository = BranchRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBranchQuery to retrieve all Branchs.
        /// </summary>
        /// <param name="request">The GetAllBranchQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BranchDto>>> Handle(GetAllBranchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Branchs entities from the repository
                var entities = await _BranchRepository.AllIncluding(c => c.Towns, x=>x.Bank).ToListAsync();
                return ServiceResponse<List<BranchDto>>.ReturnResultWith200(_mapper.Map<List<BranchDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Branchs: {e.Message}");
                return ServiceResponse<List<BranchDto>>.Return500(e, "Failed to get all Branchs");
            }
        }
    }
}
