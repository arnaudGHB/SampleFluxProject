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
    public class GetPrimaryTellerProvisioningHistoryQueryHandler : IRequestHandler<GetPrimaryTellerProvisioningHistoryQuery, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _PrimaryTellerProvisioningHistoryRepository; // Repository for accessing PrimaryTellerProvisioningHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetPrimaryTellerProvisioningHistoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetPrimaryTellerProvisioningHistoryQueryHandler.
        /// </summary>
        /// <param name="PrimaryTellerProvisioningHistoryRepository">Repository for PrimaryTellerProvisioningHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetPrimaryTellerProvisioningHistoryQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository PrimaryTellerProvisioningHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetPrimaryTellerProvisioningHistoryQueryHandler> logger)
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
        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(GetPrimaryTellerProvisioningHistoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the PrimaryTellerProvisioningHistory entity with the specified ID from the repository
                var entity = await _PrimaryTellerProvisioningHistoryRepository.FindBy(a => a.Id == request.Id).Include(x => x.Teller).Include(x => x.Teller.Transactions).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Check if the ClossedDate is null and set it to a new date if it is null
                    if (entity.ClossedDate == null)
                    {
                        entity.ClossedDate = DateTime.Now; // or any other default date you prefer
                    }

                    // Map the PrimaryTellerProvisioningHistory entity to PrimaryTellerProvisioningHistoryDto and return it with a success response
                    var PrimaryTellerProvisioningHistoryDto = _mapper.Map<PrimaryTellerProvisioningHistoryDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.ReturnResultWith200(PrimaryTellerProvisioningHistoryDto);
                }
                else
                {
                    // If the PrimaryTellerProvisioningHistory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("PrimaryTellerProvisioningHistory not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "PrimaryTellerProvisioningHistory not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting PrimaryTellerProvisioningHistory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(e);
            }
        }


    }

}
