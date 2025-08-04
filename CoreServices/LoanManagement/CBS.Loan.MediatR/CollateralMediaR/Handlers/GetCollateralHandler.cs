using AutoMapper;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CollateralMediaR.Queries;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetCollateralHandler : IRequestHandler<GetCollateralQuery, ServiceResponse<CollateralDto>>
    {
        private readonly ICollateralRepository _CollateralRepository; // Repository for accessing Collateral data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCollateralHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCollateralQueryHandler.
        /// </summary>
        /// <param name="CollateralRepository">Repository for Collateral data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCollateralHandler(
            ICollateralRepository CollateralRepository,
            IMapper mapper,
            ILogger<GetCollateralHandler> logger)
        {
            _CollateralRepository = CollateralRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCollateralQuery to retrieve a specific Collateral.
        /// </summary>
        /// <param name="request">The GetCollateralQuery containing Collateral ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CollateralDto>> Handle(GetCollateralQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Collateral entity with the specified ID from the repository
                var entity = await _CollateralRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Collateral entity to CollateralDto and return it with a success response
                    var CollateralDto = _mapper.Map<CollateralDto>(entity);
                    return ServiceResponse<CollateralDto>.ReturnResultWith200(CollateralDto);
                }
                else
                {
                    // If the Collateral entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Collateral not found.");
                    return ServiceResponse<CollateralDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Collateral: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CollateralDto>.Return500(e);
            }
        }
    }

}
