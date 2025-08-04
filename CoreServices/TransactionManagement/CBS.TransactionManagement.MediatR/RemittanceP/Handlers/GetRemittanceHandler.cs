using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Repository.RemittanceP;
using CBS.TransactionManagement.MediatR.RemittanceP.Queries;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Remittance based on its unique identifier.
    /// </summary>
    public class GetRemittanceHandler : IRequestHandler<GetRemittanceQuery, ServiceResponse<RemittanceDto>>
    {
        private readonly IRemittanceRepository _remittanceRepository; // Repository for accessing Remittance data.
        private readonly IMapper _mapper; // AutoMapper for mapping objects.
        private readonly ILogger<GetRemittanceHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken; // Token containing user info for auditing purposes.

        /// <summary>
        /// Constructor for initializing the GetRemittanceHandler.
        /// </summary>
        /// <param name="remittanceRepository">Repository for accessing Remittance data.</param>
        /// <param name="userInfoToken">User information token for logging and auditing.</param>
        /// <param name="mapper">AutoMapper for mapping objects.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        public GetRemittanceHandler(
            IRemittanceRepository remittanceRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetRemittanceHandler> logger)
        {
            _remittanceRepository = remittanceRepository ?? throw new ArgumentNullException(nameof(remittanceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the request to retrieve a specific Remittance.
        /// </summary>
        /// <param name="request">The query containing the ID of the Remittance to retrieve.</param>
        /// <param name="cancellationToken">Token to observe for cancellation.</param>
        /// <returns>A service response containing the Remittance DTO or an appropriate error.</returns>
        public async Task<ServiceResponse<RemittanceDto>> Handle(GetRemittanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Log start of retrieval process
                _logger.LogInformation($"Attempting to retrieve Remittance with ID: {request.Id}");

                // Retrieve the Remittance entity by ID
                var entity = await _remittanceRepository.FindAsync(request.Id);

                if (entity != null)
                {
                    // Map entity to DTO and return a successful response
                    var remittanceDto = _mapper.Map<RemittanceDto>(entity);
                    _logger.LogInformation($"Successfully retrieved Remittance with reference: {entity.TransactionReference}");

                    await APICallHelper.AuditLogger(
                        _userInfoToken.Email,
                        LogAction.Read.ToString(),
                        request,
                        "Success",
                        LogLevelInfo.Information.ToString(),
                        200,
                        _userInfoToken.Token);

                    return ServiceResponse<RemittanceDto>.ReturnResultWith200(remittanceDto);
                }

                // Log and return not found response if entity does not exist
                string notFoundMessage = $"Remittance with ID {request.Id} not found.";
                _logger.LogWarning(notFoundMessage);

                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Read.ToString(),
                    request,
                    notFoundMessage,
                    LogLevelInfo.Warning.ToString(),
                    404,
                    _userInfoToken.Token);

                return ServiceResponse<RemittanceDto>.Return404(notFoundMessage);
            }
            catch (Exception ex)
            {
                // Log and audit the exception
                string errorMessage = $"An error occurred while retrieving Remittance with ID {request.Id}: {ex.Message}";
                _logger.LogError(errorMessage);

                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Read.ToString(),
                    request,
                    errorMessage,
                    LogLevelInfo.Error.ToString(),
                    500,
                    _userInfoToken.Token);

                return ServiceResponse<RemittanceDto>.Return500(ex);
            }
        }
    }
}
