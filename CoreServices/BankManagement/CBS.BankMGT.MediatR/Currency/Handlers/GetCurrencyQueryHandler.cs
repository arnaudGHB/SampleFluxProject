using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Currency based on its unique identifier.
    /// </summary>
    public class GetCurrencyQueryHandler : IRequestHandler<GetCurrencyQuery, ServiceResponse<CurrencyDto>>
    {
        private readonly ICurrencyRepository _CurrencyRepository; // Repository for accessing Currency data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCurrencyQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCurrencyQueryHandler.
        /// </summary>
        /// <param name="CurrencyRepository">Repository for Currency data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCurrencyQueryHandler(
            ICurrencyRepository CurrencyRepository,
            IMapper mapper,
            ILogger<GetCurrencyQueryHandler> logger)
        {
            _CurrencyRepository = CurrencyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCurrencyQuery to retrieve a specific Currency.
        /// </summary>
        /// <param name="request">The GetCurrencyQuery containing Currency ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CurrencyDto>> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Currency entity with the specified ID from the repository
                var entity = await _CurrencyRepository.AllIncluding(c => c.Country).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (entity != null)
                {
                    // Map the Currency entity to CurrencyDto and return it with a success response
                    var CurrencyDto = _mapper.Map<CurrencyDto>(entity);
                    return ServiceResponse<CurrencyDto>.ReturnResultWith200(CurrencyDto);
                }
                else
                {
                    // If the Currency entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Currency not found.");
                    return ServiceResponse<CurrencyDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Currency: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CurrencyDto>.Return500(e);
            }
        }
    }

}
