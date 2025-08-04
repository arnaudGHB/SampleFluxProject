using AutoMapper;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AlertProfileMediaR.Queries;
using CBS.NLoan.Repository.AlertProfileP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllAlertProfileHandler : IRequestHandler<GetAllAlertProfileQuery, ServiceResponse<List<AlertProfileDto>>>
    {
        private readonly IAlertProfileRepository _AlertProfileRepository; // Repository for accessing AlertProfiles data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAlertProfileHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAlertProfileQueryHandler.
        /// </summary>
        /// <param name="AlertProfileRepository">Repository for AlertProfiles data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAlertProfileHandler(
            IAlertProfileRepository AlertProfileRepository,
            IMapper mapper, ILogger<GetAllAlertProfileHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AlertProfileRepository = AlertProfileRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAlertProfileQuery to retrieve all AlertProfiles.
        /// </summary>
        /// <param name="request">The GetAllAlertProfileQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AlertProfileDto>>> Handle(GetAllAlertProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AlertProfiles entities from the repository
                var entities = await _AlertProfileRepository.All.ToListAsync();
                return ServiceResponse<List<AlertProfileDto>>.ReturnResultWith200(_mapper.Map<List<AlertProfileDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all AlertProfiles: {e.Message}");
                return ServiceResponse<List<AlertProfileDto>>.Return500(e, "Failed to get all AlertProfiles");
            }
        }
    }
}
