using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all SavingProduct based on the GetAllSavingProductQuery.
    /// </summary>
    public class GetAllSavingProductQueryHandler : IRequestHandler<GetAllSavingProductQuery, ServiceResponse<List<SavingProductDto>>>
    {
        private readonly ISavingProductRepository _SavingProductRepository; // Repository for accessing SavingProducts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSavingProductQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllSavingProductQueryHandler.
        /// </summary>
        /// <param name="SavingProductRepository">Repository for SavingProducts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSavingProductQueryHandler(
            ISavingProductRepository SavingProductRepository,
            IMapper mapper, ILogger<GetAllSavingProductQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllSavingProductQuery to retrieve all SavingProducts.
        /// </summary>
        /// <param name="request">The GetAllSavingProductQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SavingProductDto>>> Handle(GetAllSavingProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all SavingProducts entities from the repository
                var entities = await _SavingProductRepository.All.AsNoTracking().ToListAsync();
                return ServiceResponse<List<SavingProductDto>>.ReturnResultWith200(_mapper.Map<List<SavingProductDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all SavingProducts: {e.Message}");
                return ServiceResponse<List<SavingProductDto>>.Return500(e, "Failed to get all SavingProducts");
            }
        }
    }
}
