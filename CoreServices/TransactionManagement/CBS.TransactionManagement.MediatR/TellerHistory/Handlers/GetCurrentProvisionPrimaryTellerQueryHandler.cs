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
    /// Handles the request to retrieve a specific TellerHistory based on its unique identifier.
    /// </summary>
    public class GetCurrentProvisionPrimaryTellerQueryHandler : IRequestHandler<GetCurrentProvisionPrimaryTellerQuery, ServiceResponse<List<CurrentProvisionPrimaryTellerDto>>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _TellerHistoryRepository; // Repository for accessing TellerHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCurrentProvisionPrimaryTellerQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetCurrentProvisionPrimaryTellerQueryHandler.
        /// </summary>
        /// <param name="TellerHistoryRepository">Repository for TellerHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCurrentProvisionPrimaryTellerQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetCurrentProvisionPrimaryTellerQueryHandler> logger)
        {
            _TellerHistoryRepository = TellerHistoryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCurrentProvisionPrimaryTellerQuery to retrieve a specific TellerHistory.
        /// </summary>
        /// <param name="request">The GetCurrentProvisionPrimaryTellerQuery containing TellerHistory ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CurrentProvisionPrimaryTellerDto>>> Handle(GetCurrentProvisionPrimaryTellerQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the TellerHistory entity with the specified ID from the repository
                var entity = await _TellerHistoryRepository.FindBy(a=>a.CreatedDate.Date == DateTime.Now.Date).ToListAsync();

                var results = (from a in entity
                               select new CurrentProvisionPrimaryTellerDto
                               {
                                   Id = a.Id,
                                   Text = $"{a.Id}-{a.OpenOfDayAmount} {a.ClossedStatus}"
                               }).ToList();
                return ServiceResponse<List<CurrentProvisionPrimaryTellerDto>>.ReturnResultWith200(results);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting TellerHistory of today: {e.Message}";
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<CurrentProvisionPrimaryTellerDto>>.Return500(e);
            }
        }

    }

}
