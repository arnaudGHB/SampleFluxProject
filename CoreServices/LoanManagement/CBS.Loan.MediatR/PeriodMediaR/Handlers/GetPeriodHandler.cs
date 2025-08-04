using AutoMapper;
using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PeriodMediaR.Queries;
using CBS.NLoan.Repository.PeriodP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PeriodMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetPeriodHandler : IRequestHandler<GetPeriodQuery, ServiceResponse<PeriodDto>>
    {
        private readonly IPeriodRepository _PeriodRepository; // Repository for accessing Period data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetPeriodHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetPeriodQueryHandler.
        /// </summary>
        /// <param name="PeriodRepository">Repository for Period data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetPeriodHandler(
            IPeriodRepository PeriodRepository,
            IMapper mapper,
            ILogger<GetPeriodHandler> logger)
        {
            _PeriodRepository = PeriodRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetPeriodQuery to retrieve a specific Period.
        /// </summary>
        /// <param name="request">The GetPeriodQuery containing Period ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PeriodDto>> Handle(GetPeriodQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Period entity with the specified ID from the repository
                var entity = await _PeriodRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Period entity to PeriodDto and return it with a success response
                    var PeriodDto = _mapper.Map<PeriodDto>(entity);
                    return ServiceResponse<PeriodDto>.ReturnResultWith200(PeriodDto);
                }
                else
                {
                    // If the Period entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Period not found.");
                    return ServiceResponse<PeriodDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Period: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<PeriodDto>.Return500(e);
            }
        }
    }

}
