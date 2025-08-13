using AutoMapper;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayP.Queries;
using CBS.TransactionManagement.Repository.HolyDayP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetHolyDayHandler : IRequestHandler<GetHolyDayQuery, ServiceResponse<HolyDayDto>>
    {
        private readonly IHolyDayRepository _HolyDayRepository; // Repository for accessing HolyDay data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetHolyDayHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetHolyDayQueryHandler.
        /// </summary>
        /// <param name="HolyDayRepository">Repository for HolyDay data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetHolyDayHandler(
            IHolyDayRepository HolyDayRepository,
            IMapper mapper,
            ILogger<GetHolyDayHandler> logger)
        {
            _HolyDayRepository = HolyDayRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetHolyDayQuery to retrieve a specific HolyDay.
        /// </summary>
        /// <param name="request">The GetHolyDayQuery containing HolyDay ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<HolyDayDto>> Handle(GetHolyDayQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the HolyDay entity with the specified ID from the repository
                var entity = await _HolyDayRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the HolyDay entity to HolyDayDto and return it with a success response
                    var HolyDayDto = _mapper.Map<HolyDayDto>(entity);
                    return ServiceResponse<HolyDayDto>.ReturnResultWith200(HolyDayDto);
                }
                else
                {
                    // If the HolyDay entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("HolyDay not found.");
                    return ServiceResponse<HolyDayDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting HolyDay: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<HolyDayDto>.Return500(e);
            }
        }
    }

}
