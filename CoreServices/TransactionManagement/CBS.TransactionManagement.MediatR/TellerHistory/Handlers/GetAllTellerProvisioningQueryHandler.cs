using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all TellerHistory based on the GetAllTellerHistoryQuery.
    /// </summary>
    public class GetAllTellerProvisioningQueryHandler : IRequestHandler<GetAllTellerProvisioningQuery, ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _TellerHistoryRepository; // Repository for accessing TellerHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTellerProvisioningQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllTellerHistoryQueryHandler.
        /// </summary>
        /// <param name="TellerHistoryRepository">Repository for TellerHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTellerProvisioningQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllTellerProvisioningQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TellerHistoryRepository = TellerHistoryRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTellerHistoryQuery to retrieve all TellerHistory.
        /// </summary>
        /// <param name="request">The GetAllTellerHistoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>> Handle(GetAllTellerProvisioningQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all TellerHistory entities from the repository
                var entities = await _TellerHistoryRepository.All.ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System TellerHistorys returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.ReturnResultWith200(_mapper.Map<List<PrimaryTellerProvisioningHistoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all TellerHistory: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all TellerHistory: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.Return500(e, "Failed to get all TellerHistory");
            }
        }
     
    }
}
