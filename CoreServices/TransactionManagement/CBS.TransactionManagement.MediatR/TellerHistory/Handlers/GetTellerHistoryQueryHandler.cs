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
    public class GetTellerHistoryQueryHandler : IRequestHandler<GetTellerHistoryQuery, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _TellerHistoryRepository; // Repository for accessing TellerHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTellerHistoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetTellerHistoryQueryHandler.
        /// </summary>
        /// <param name="TellerHistoryRepository">Repository for TellerHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTellerHistoryQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetTellerHistoryQueryHandler> logger)
        {
            _TellerHistoryRepository = TellerHistoryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTellerHistoryQuery to retrieve a specific TellerHistory.
        /// </summary>
        /// <param name="request">The GetTellerHistoryQuery containing TellerHistory ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(GetTellerHistoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the TellerHistory entity with the specified ID from the repository
                var entity = await _TellerHistoryRepository.FindBy(a=>a.Id == request.Id).FirstAsync();
                if (entity != null)
                {
                    // Map the TellerHistory entity to TellerHistoryDto and return it with a success response
                    var TellerHistoryDto = _mapper.Map<PrimaryTellerProvisioningHistoryDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.ReturnResultWith200(TellerHistoryDto);
                }
                else
                {
                    // If the TellerHistory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("TellerHistory not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "TellerHistory not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting TellerHistory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(e);
            }
        }

    }

}
