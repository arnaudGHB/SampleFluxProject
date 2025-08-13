using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.DailyTellerP.Queries;

namespace CBS.TransactionManagement.DailyTellerP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all DailyTeller based on the GetDailyTellerByUserIdQuery.
    /// </summary>
    public class GetDailyTellerByUserIdQueryHandler : IRequestHandler<GetDailyTellerByUserIdQuery, ServiceResponse<DailyTellerDto>>
    {
        private readonly IDailyTellerRepository _DailyTellerRepository; // Repository for accessing DailyTellers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDailyTellerByUserIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDailyTellerByUserIdQueryHandler.
        /// </summary>
        /// <param name="DailyTellerRepository">Repository for DailyTellers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDailyTellerByUserIdQueryHandler(
            IDailyTellerRepository DailyTellerRepository,
            IMapper mapper, ILogger<GetDailyTellerByUserIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DailyTellerRepository = DailyTellerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDailyTellerByUserIdQuery to retrieve the DailyTeller based on UserId and TellerType.
        /// </summary>
        /// <param name="request">The GetDailyTellerByUserIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DailyTellerDto>> Handle(GetDailyTellerByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Ensure UserId is provided
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    return ServiceResponse<DailyTellerDto>.Return400("UserId is required.");
                }

                // Ensure TellerType is provided
                if (string.IsNullOrWhiteSpace(request.TellerType))
                {
                    return ServiceResponse<DailyTellerDto>.Return400("TellerType is required.");
                }

                // Convert TellerType to boolean for query filtering
                bool isPrimary = request.TellerType.Equals("Primary", StringComparison.OrdinalIgnoreCase);

                // Query the DailyTeller entity based on parameters
                var entity = await _DailyTellerRepository
                    .FindBy(x => !x.IsDeleted && x.UserId == request.UserId && x.IsPrimary == isPrimary)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return ServiceResponse<DailyTellerDto>.Return404($"No DailyTeller found for UserId '{request.UserId}' with TellerType '{request.TellerType}'.");
                }

                // Map the entity to a DTO
                var result = _mapper.Map<DailyTellerDto>(entity);

                return ServiceResponse<DailyTellerDto>.ReturnResultWith200(result);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response
                _logger.LogError($"Failed to retrieve DailyTeller: {e.Message}");
                return ServiceResponse<DailyTellerDto>.Return500(e, "Failed to retrieve the DailyTeller.");
            }
        }

    }
}
