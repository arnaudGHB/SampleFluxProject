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
    /// Handles the retrieval of all SubTellerProvioningHistory based on the GetAllSubTellerProvioningHistoryBetweenDatesQuery.
    /// </summary>
    public class GetAllSubTellerProvioningHistoryBetweenDatesQueryHandler : IRequestHandler<GetAllSubTellerProvioningHistoryBetweenDatesQuery, ServiceResponse<List<SubTellerProvioningHistoryDto>>>
    {
        private readonly ISubTellerProvisioningHistoryRepository _SubTellerProvioningHistoryRepository; // Repository for accessing SubTellerProvioningHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSubTellerProvioningHistoryBetweenDatesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllSubTellerProvioningHistoryBetweenDatesQueryHandler.
        /// </summary>
        /// <param name="SubTellerProvioningHistoryRepository">Repository for SubTellerProvioningHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSubTellerProvioningHistoryBetweenDatesQueryHandler(
            ISubTellerProvisioningHistoryRepository SubTellerProvioningHistoryRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllSubTellerProvioningHistoryBetweenDatesQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _SubTellerProvioningHistoryRepository = SubTellerProvioningHistoryRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllSubTellerProvioningHistoryBetweenDatesQuery to retrieve all SubTellerProvioningHistory.
        /// </summary>
        /// <param name="request">The GetAllSubTellerProvioningHistoryBetweenDatesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SubTellerProvioningHistoryDto>>> Handle(GetAllSubTellerProvioningHistoryBetweenDatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all SubTellerProvioningHistory entities from the repository
                var entities = await _SubTellerProvioningHistoryRepository.FindByDateRange(x => x.CreatedDate, request.DateFrom, request.DateTo).Include(x => x.Teller).ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System SubTellerProvioningHistorys returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<SubTellerProvioningHistoryDto>>.ReturnResultWith200(_mapper.Map<List<SubTellerProvioningHistoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all SubTellerProvioningHistory: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all SubTellerProvioningHistory: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<SubTellerProvioningHistoryDto>>.Return500(e, "Failed to get all SubTellerProvioningHistory");
            }
        }

    }
}
