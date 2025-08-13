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
    /// Handles the retrieval of all PrimaryTellerProvisioningHistory based on the GetAllPrimaryTellerProvisioningHistoryQuery.
    /// </summary>
    public class GetAllPrimaryTellerProvisioningHistoryQueryHandler : IRequestHandler<GetAllPrimaryTellerProvisioningHistoryQuery, ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _PrimaryTellerProvisioningHistoryRepository; // Repository for accessing PrimaryTellerProvisioningHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllPrimaryTellerProvisioningHistoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllPrimaryTellerProvisioningHistoryQueryHandler.
        /// </summary>
        /// <param name="PrimaryTellerProvisioningHistoryRepository">Repository for PrimaryTellerProvisioningHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllPrimaryTellerProvisioningHistoryQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository PrimaryTellerProvisioningHistoryRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllPrimaryTellerProvisioningHistoryQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _PrimaryTellerProvisioningHistoryRepository = PrimaryTellerProvisioningHistoryRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllPrimaryTellerProvisioningHistoryQuery to retrieve all PrimaryTellerProvisioningHistory.
        /// </summary>
        /// <param name="request">The GetAllPrimaryTellerProvisioningHistoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>> Handle(GetAllPrimaryTellerProvisioningHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all PrimaryTellerProvisioningHistory entities from the repository
                var entities = await _PrimaryTellerProvisioningHistoryRepository.AllIncluding(x => x.Teller).Include(x => x.Teller.Transactions).ToListAsync();

                // Set default date if any date property is null
                foreach (var entity in entities)
                {
                    entity.OpenedDate ??= new DateTime(1900, 1, 1);
                    entity.ClossedDate ??= new DateTime(1900, 1, 1);
                    // Add other date properties here if needed
                }

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

            //try
            //{
            //    // Retrieve all PrimaryTellerProvisioningHistory entities from the repository
            //    var entities = await _PrimaryTellerProvisioningHistoryRepository.AllIncluding(x=>x.Teller).ToListAsync();
            //    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System PrimaryTellerProvisioningHistorys returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
            //    return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.ReturnResultWith200(_mapper.Map<List<PrimaryTellerProvisioningHistoryDto>>(entities));
            //}
            //catch (Exception e)
            //{
            //    // Log error and return a 500 Internal Server Error response with error message
            //    _logger.LogError($"Failed to get all PrimaryTellerProvisioningHistory: {e.Message}");
            //    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all PrimaryTellerProvisioningHistory: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
            //    return ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>.Return500(e, "Failed to get all PrimaryTellerProvisioningHistory");
            //}
        }

    }
}
