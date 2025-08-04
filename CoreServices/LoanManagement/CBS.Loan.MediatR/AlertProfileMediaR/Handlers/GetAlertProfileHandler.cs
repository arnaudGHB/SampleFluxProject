using AutoMapper;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AlertProfileMediaR.Queries;
using CBS.NLoan.Repository.AlertProfileP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetAlertProfileHandler : IRequestHandler<GetAlertProfileQuery, ServiceResponse<AlertProfileDto>>
    {
        private readonly IAlertProfileRepository _AlertProfileRepository; // Repository for accessing AlertProfile data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAlertProfileHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAlertProfileQueryHandler.
        /// </summary>
        /// <param name="AlertProfileRepository">Repository for AlertProfile data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAlertProfileHandler(
            IAlertProfileRepository AlertProfileRepository,
            IMapper mapper,
            ILogger<GetAlertProfileHandler> logger)
        {
            _AlertProfileRepository = AlertProfileRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAlertProfileQuery to retrieve a specific AlertProfile.
        /// </summary>
        /// <param name="request">The GetAlertProfileQuery containing AlertProfile ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AlertProfileDto>> Handle(GetAlertProfileQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AlertProfile entity with the specified ID from the repository
                var entity = await _AlertProfileRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the AlertProfile entity to AlertProfileDto and return it with a success response
                    var AlertProfileDto = _mapper.Map<AlertProfileDto>(entity);
                    return ServiceResponse<AlertProfileDto>.ReturnResultWith200(AlertProfileDto);
                }
                else
                {
                    // If the AlertProfile entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AlertProfile not found.");
                    return ServiceResponse<AlertProfileDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AlertProfile: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AlertProfileDto>.Return500(e);
            }
        }
    }

}
