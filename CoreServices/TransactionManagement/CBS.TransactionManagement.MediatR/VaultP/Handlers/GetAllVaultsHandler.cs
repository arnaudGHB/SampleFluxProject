using AutoMapper;
using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.MediatR.VaultP.Queries;
using CBS.TransactionManagement.Repository.VaultP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.MediatR.VaultP
{


    /// <summary>
    /// Handles retrieval of all vaults.
    /// </summary>
    public class GetAllVaultsHandler : IRequestHandler<GetAllVaultsQuery, ServiceResponse<List<VaultDto>>>
    {
        private readonly IVaultRepository _vaultRepository; // Repository for vault operations.
        private readonly ILogger<GetAllVaultsHandler> _logger; // Logger for recording events and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllVaultsHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The repository for managing vaults.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public GetAllVaultsHandler(IVaultRepository vaultRepository, ILogger<GetAllVaultsHandler> logger, IMapper mapper)
        {
            _vaultRepository = vaultRepository; // Inject the vault repository.
            _logger = logger; // Inject the logger for logging.
            _mapper = mapper; // Inject AutoMapper for mapping objects.
        }

        /// <summary>
        /// Handles the query to get all vaults.
        /// </summary>
        /// <param name="request">The query object (not used for filtering).</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A response containing the list of all vaults.</returns>
        public async Task<ServiceResponse<List<VaultDto>>> Handle(GetAllVaultsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all vaults that are not marked as deleted.
                var vaults = await _vaultRepository.FindBy(v => !v.IsDeleted).ToListAsync();

                // Step 2: Check if no vaults were found.
                if (vaults == null || vaults.Count == 0)
                {
                    const string warningMessage = "No vaults found in the system.";
                    _logger.LogWarning(warningMessage); // Log a warning if no vaults were found.

                    // Log the warning using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        warningMessage,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.VaultRetrieval,
                        LogLevelInfo.Warning);

                    return ServiceResponse<List<VaultDto>>.Return404(warningMessage); // Return 404 Not Found response.
                }

                // Step 3: Map the vault entities to VaultDto list.
                var vaultDtos = _mapper.Map<List<VaultDto>>(vaults);

                // Step 4: Log successful retrieval of vaults.
                string successMessage = $"Successfully retrieved {vaultDtos.Count} vault(s) from the system.";
                _logger.LogInformation(successMessage); // Log the success message.

                // Log the success using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultRetrieval,
                    LogLevelInfo.Information);

                // Step 5: Return a 200 OK response with the list of vaults.
                return ServiceResponse<List<VaultDto>>.ReturnResultWith200(vaultDtos, "Vaults retrieved successfully.");
            }
            catch (Exception ex)
            {
                // Step 6: Handle and log unexpected errors.
                string errorMessage = $"An unexpected error occurred while retrieving all vaults. Error: {ex.Message}";
                _logger.LogError(errorMessage); // Log the exception details.

                // Log the error using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultRetrieval,
                    LogLevelInfo.Error);

                // Step 7: Return a 500 Internal Server Error response.
                return ServiceResponse<List<VaultDto>>.Return500(errorMessage);
            }
        }
    }


}
