using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Queries;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific PrimaryTellerProvisioningHistory based on its unique identifier.
    /// </summary>
    public class GetPrimaryTellerProvisioningHistoryByUserIDQueryHandler : IRequestHandler<GetPrimaryTellerProvisioningHistoryByUserIDQuery, ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _PrimaryTellerProvisioningHistoryRepository; // Repository for accessing PrimaryTellerProvisioningHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetPrimaryTellerProvisioningHistoryByUserIDQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetPrimaryTellerProvisioningHistoryByUserIDQueryHandler.
        /// </summary>
        /// <param name="PrimaryTellerProvisioningHistoryRepository">Repository for PrimaryTellerProvisioningHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetPrimaryTellerProvisioningHistoryByUserIDQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository PrimaryTellerProvisioningHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetPrimaryTellerProvisioningHistoryByUserIDQueryHandler> logger)
        {
            _PrimaryTellerProvisioningHistoryRepository = PrimaryTellerProvisioningHistoryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetPrimaryTellerProvisioningHistoryQuery to retrieve a specific PrimaryTellerProvisioningHistory.
        /// </summary>
        /// <param name="request">The GetPrimaryTellerProvisioningHistoryQuery containing PrimaryTellerProvisioningHistory ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>> Handle(GetPrimaryTellerProvisioningHistoryByUserIDQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the PrimaryTellerProvisioningHistory entity with the specified ID from the repository
                var entity = await _PrimaryTellerProvisioningHistoryRepository.FindBy(a => a.UserIdInChargeOfThisTeller == request.UserInchargeId).Include(x => x.Teller).ToListAsync();
                if (entity != null)
                {
                    // Map the PrimaryTellerProvisioningHistory entity to PrimaryTellerProvisioningHistoryDto and return it with a success response
                    var PrimaryTellerProvisioningHistoryDto = _mapper.Map<List<PrimaryTellerProvisioningHistoryDto>>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.ReturnResultWith200(PrimaryTellerProvisioningHistoryDto);
                }
                else
                {
                    // If the PrimaryTellerProvisioningHistory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("PrimaryTellerProvisioningHistory not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "PrimaryTellerProvisioningHistory not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting PrimaryTellerProvisioningHistory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.Return500(e);
            }
        }

    }

}
