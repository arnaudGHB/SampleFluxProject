using AutoMapper;
using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.TaxMediaR.Queries;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.TaxMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllTaxHandler : IRequestHandler<GetAllTaxQuery, ServiceResponse<List<TaxDto>>>
    {
        private readonly ITaxRepository _TaxRepository; // Repository for accessing Taxs data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTaxHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTaxQueryHandler.
        /// </summary>
        /// <param name="TaxRepository">Repository for Taxs data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTaxHandler(
            ITaxRepository TaxRepository,
            IMapper mapper, ILogger<GetAllTaxHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TaxRepository = TaxRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTaxQuery to retrieve all Taxs.
        /// </summary>
        /// <param name="request">The GetAllTaxQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TaxDto>>> Handle(GetAllTaxQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Taxs entities from the repository
                var entities = await _TaxRepository.All.Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<TaxDto>>.ReturnResultWith200(_mapper.Map<List<TaxDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Taxs: {e.Message}");
                return ServiceResponse<List<TaxDto>>.Return500(e, "Failed to get all Taxs");
            }
        }
    }
}
