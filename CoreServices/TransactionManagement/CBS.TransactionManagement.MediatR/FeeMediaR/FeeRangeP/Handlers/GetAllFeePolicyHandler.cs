using AutoMapper;
using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FeeP.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FeePolicyP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllFeePolicyHandler : IRequestHandler<GetAllFeePolicyQuery, ServiceResponse<List<FeePolicyDto>>>
    {
        private readonly IFeePolicyRepository _FeePolicyRepository; // Repository for accessing Fees data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFeePolicyHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllFeePolicyHandler.
        /// </summary>
        /// <param name="FeePolicyRepository">Repository for Fees data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFeePolicyHandler(
            IFeePolicyRepository FeePolicyRepository,
            IMapper mapper,
            ILogger<GetAllFeePolicyHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _FeePolicyRepository = FeePolicyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllFeePolicyQuery to retrieve all Fees.
        /// </summary>
        /// <param name="request">The GetAllFeePolicyQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FeePolicyDto>>> Handle(GetAllFeePolicyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all FeePolicy entities from the repository
                var entities = await _FeePolicyRepository.All.Include(x=>x.Fee).Where(x => !x.IsDeleted).ToListAsync();
                var dtos = _mapper.Map<List<FeePolicyDto>>(entities);
                return ServiceResponse<List<FeePolicyDto>>.ReturnResultWith200(dtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Fee ranges: {e.Message}");
                return ServiceResponse<List<FeePolicyDto>>.Return500(e, "Failed to get all Fee ranges");
            }
        }
    }
}
