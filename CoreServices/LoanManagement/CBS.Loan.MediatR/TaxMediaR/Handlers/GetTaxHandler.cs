using AutoMapper;
using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.TaxMediaR.Queries;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.TaxMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetTaxHandler : IRequestHandler<GetTaxQuery, ServiceResponse<TaxDto>>
    {
        private readonly ITaxRepository _TaxRepository; // Repository for accessing Tax data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTaxHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetTaxQueryHandler.
        /// </summary>
        /// <param name="TaxRepository">Repository for Tax data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTaxHandler(
            ITaxRepository TaxRepository,
            IMapper mapper,
            ILogger<GetTaxHandler> logger)
        {
            _TaxRepository = TaxRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTaxQuery to retrieve a specific Tax.
        /// </summary>
        /// <param name="request">The GetTaxQuery containing Tax ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TaxDto>> Handle(GetTaxQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Tax entity with the specified ID from the repository
                var entity = await _TaxRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Tax entity to TaxDto and return it with a success response
                    var TaxDto = _mapper.Map<TaxDto>(entity);
                    return ServiceResponse<TaxDto>.ReturnResultWith200(TaxDto);
                }
                else
                {
                    // If the Tax entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Tax not found.");
                    return ServiceResponse<TaxDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Tax: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TaxDto>.Return500(e);
            }
        }
    }

}
