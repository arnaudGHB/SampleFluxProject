using AutoMapper;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Queries;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringMediaR.HolyDayRecurringP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetHolyDayRecurringHandler : IRequestHandler<GetHolyDayRecurringQuery, ServiceResponse<HolyDayRecurringDto>>
    {
        private readonly IHolyDayRecurringRepository _HolyDayRecurringRepository; // Repository for accessing HolyDayRecurring data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetHolyDayRecurringHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetHolyDayRecurringQueryHandler.
        /// </summary>
        /// <param name="HolyDayRecurringRepository">Repository for HolyDayRecurring data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetHolyDayRecurringHandler(
            IHolyDayRecurringRepository HolyDayRecurringRepository,
            IMapper mapper,
            ILogger<GetHolyDayRecurringHandler> logger)
        {
            _HolyDayRecurringRepository = HolyDayRecurringRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetHolyDayRecurringQuery to retrieve a specific HolyDayRecurring.
        /// </summary>
        /// <param name="request">The GetHolyDayRecurringQuery containing HolyDayRecurring ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<HolyDayRecurringDto>> Handle(GetHolyDayRecurringQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the HolyDayRecurring entity with the specified ID from the repository
                var entity = await _HolyDayRecurringRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the HolyDayRecurring entity to HolyDayRecurringDto and return it with a success response
                    var HolyDayRecurringDto = _mapper.Map<HolyDayRecurringDto>(entity);
                    return ServiceResponse<HolyDayRecurringDto>.ReturnResultWith200(HolyDayRecurringDto);
                }
                else
                {
                    // If the HolyDayRecurring entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("HolyDayRecurring not found.");
                    return ServiceResponse<HolyDayRecurringDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting HolyDayRecurring: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<HolyDayRecurringDto>.Return500(e);
            }
        }
    }

}
