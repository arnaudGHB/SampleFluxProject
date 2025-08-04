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
    /// Handles the retrieval of all Currencys based on the GetAllCurrencyQuery.
    /// </summary>
    public class GetAllCurrencyQueryHandler : IRequestHandler<GetAllCurrencyQuery, ServiceResponse<List<CurrencyDto>>>
    {
        private readonly ICurrencyRepository _CurrencyRepository; // Repository for accessing Currencys data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCurrencyQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCurrencyQueryHandler.
        /// </summary>
        /// <param name="CurrencyRepository">Repository for Currencys data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCurrencyQueryHandler(
            ICurrencyRepository CurrencyRepository,
            IMapper mapper, ILogger<GetAllCurrencyQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CurrencyRepository = CurrencyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCurrencyQuery to retrieve all Currencys.
        /// </summary>
        /// <param name="request">The GetAllCurrencyQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CurrencyDto>>> Handle(GetAllCurrencyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Currencys entities from the repository
                var entities = await _CurrencyRepository.AllIncluding(c => c.Country).ToListAsync();
                return ServiceResponse<List<CurrencyDto>>.ReturnResultWith200(_mapper.Map<List<CurrencyDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Currencys: {e.Message}");
                return ServiceResponse<List<CurrencyDto>>.Return500(e, "Failed to get all Currencys");
            }
        }
    }
}
