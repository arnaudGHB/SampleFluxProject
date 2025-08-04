using AutoMapper;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CollateralMediaR.Queries;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllCollateralHandler : IRequestHandler<GetAllCollateralQuery, ServiceResponse<List<CollateralDto>>>
    {
        private readonly ICollateralRepository _CollateralRepository; // Repository for accessing Collaterals data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCollateralHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCollateralQueryHandler.
        /// </summary>
        /// <param name="CollateralRepository">Repository for Collaterals data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCollateralHandler(
            ICollateralRepository CollateralRepository,
            IMapper mapper, ILogger<GetAllCollateralHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CollateralRepository = CollateralRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCollateralQuery to retrieve all Collaterals.
        /// </summary>
        /// <param name="request">The GetAllCollateralQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CollateralDto>>> Handle(GetAllCollateralQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Collaterals entities from the repository
                var entities = await _CollateralRepository.All.Where(x => !x.IsDeleted)
                   .ToListAsync();
                return ServiceResponse<List<CollateralDto>>.ReturnResultWith200(_mapper.Map<List<CollateralDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Collaterals: {e.Message}");
                return ServiceResponse<List<CollateralDto>>.Return500(e, "Failed to get all Collaterals");
            }
        }
    }
}
