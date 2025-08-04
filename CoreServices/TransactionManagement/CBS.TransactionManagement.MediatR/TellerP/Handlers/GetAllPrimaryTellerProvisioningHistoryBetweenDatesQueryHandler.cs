using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all PrimaryTellerProvisioningHistory based on the GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery.
    /// </summary>
    public class GetAllPrimaryTellerProvisioningHistoryBetweenDatesQueryHandler : IRequestHandler<GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery, ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _PrimaryTellerProvisioningHistoryRepository; // Repository for accessing PrimaryTellerProvisioningHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllPrimaryTellerProvisioningHistoryBetweenDatesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllPrimaryTellerProvisioningHistoryBetweenDatesQueryHandler.
        /// </summary>
        /// <param name="PrimaryTellerProvisioningHistoryRepository">Repository for PrimaryTellerProvisioningHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllPrimaryTellerProvisioningHistoryBetweenDatesQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository PrimaryTellerProvisioningHistoryRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllPrimaryTellerProvisioningHistoryBetweenDatesQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _PrimaryTellerProvisioningHistoryRepository = PrimaryTellerProvisioningHistoryRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery to retrieve all PrimaryTellerProvisioningHistory.
        /// </summary>
        /// <param name="request">The GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>> Handle(GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all PrimaryTellerProvisioningHistory entities from the repository
                var entities = await _PrimaryTellerProvisioningHistoryRepository.FindByDateRange(x => x.CreatedDate, request.DateFrom, request.DateTo).Include(x => x.Teller).ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System PrimaryTellerProvisioningHistorys returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.ReturnResultWith200(_mapper.Map<List<PrimaryTellerProvisioningHistoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all PrimaryTellerProvisioningHistory: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all PrimaryTellerProvisioningHistory: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.Return500(e, "Failed to get all PrimaryTellerProvisioningHistory");
            }
        }

    }
}
