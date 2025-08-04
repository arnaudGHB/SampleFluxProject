using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific SavingProduct based on its unique identifier.
    /// </summary>
    public class GetSavingProductQueryHandler : IRequestHandler<GetSavingProductQuery, ServiceResponse<SavingProductDto>>
    {
        private readonly ISavingProductRepository _SavingProductRepository; // Repository for accessing SavingProduct data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSavingProductQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetSavingProductQueryHandler.
        /// </summary>
        /// <param name="SavingProductRepository">Repository for SavingProduct data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSavingProductQueryHandler(
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            ILogger<GetSavingProductQueryHandler> logger)
        {
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetSavingProductQuery to retrieve a specific SavingProduct.
        /// </summary>
        /// <param name="request">The GetSavingProductQuery containing SavingProduct ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SavingProductDto>> Handle(GetSavingProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _SavingProductRepository.FindAsync(request.Id);

                if (entity != null)
                {
                    // Map the SavingProduct entity to SavingProductDto and return it with a success response
                    var SavingProductDto = _mapper.Map<SavingProductDto>(entity);
                    return ServiceResponse<SavingProductDto>.ReturnResultWith200(SavingProductDto);
                }

                // If the SavingProduct entity was not found, log the error and return a 404 Not Found response
                _logger.LogError("SavingProduct not found.");
                return ServiceResponse<SavingProductDto>.Return404();
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError($"Error occurred while getting SavingProduct: {e.Message}");
                return ServiceResponse<SavingProductDto>.Return500(e);
            }
        }

    }

}
