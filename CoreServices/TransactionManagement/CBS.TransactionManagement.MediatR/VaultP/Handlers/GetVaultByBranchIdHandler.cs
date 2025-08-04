using AutoMapper;
using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.MediatR.VaultP.Queries;
using CBS.TransactionManagement.Repository.VaultOperationP;
using CBS.TransactionManagement.Repository.VaultP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.MediatR.VaultP
{

    /// <summary>
    /// Handles retrieval of a vault by BranchId.
    /// </summary>
    public class GetVaultByBranchIdHandler : IRequestHandler<GetVaultByBranchIdQuery, ServiceResponse<VaultDto>>
    {
        private readonly IVaultRepository _vaultRepository; // Repository for vault operations.
        private readonly ILogger<GetVaultByBranchIdHandler> _logger; // Logger for recording events and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IVaultOperationRepository _vaultOperationRepository; // Repository for managing vaults.

        /// <summary>
        /// Initializes a new instance of the <see cref="GetVaultByBranchIdHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The repository for managing vaults.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public GetVaultByBranchIdHandler(IVaultRepository vaultRepository, ILogger<GetVaultByBranchIdHandler> logger, IMapper mapper, IVaultOperationRepository vaultOperationRepository)
        {
            _vaultRepository = vaultRepository; // Inject the vault repository.
            _logger = logger; // Inject the logger for logging.
            _mapper = mapper; // Inject AutoMapper for mapping objects.
            _vaultOperationRepository=vaultOperationRepository;
        }

        /// <summary>
        /// Handles the query to get a vault by BranchId.
        /// </summary>
        /// <param name="request">The query containing the BranchId.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A response containing the vault details.</returns>
        public async Task<ServiceResponse<VaultDto>> Handle(GetVaultByBranchIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate input parameters.
                if (string.IsNullOrWhiteSpace(request.BranchId))
                {
                    const string error = "BranchId is a required field."; // Validation error message.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultRetrieval,
                        LogLevelInfo.Error);

                    return ServiceResponse<VaultDto>.Return400(error); // Return 400 Bad Request response.
                }

                // Step 2: Retrieve the vault by BranchId.
                var vault = await _vaultRepository.FindBy(v => v.BranchId == request.BranchId && !v.IsDeleted).FirstOrDefaultAsync();

                if (vault == null)
                {
                    const string error = "No vault found for the specified BranchId."; // Error message for missing vault.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.VaultRetrieval,
                        LogLevelInfo.Warning);

                    return ServiceResponse<VaultDto>.Return404(error); // Return 404 Not Found response.
                }

                // Step 3: Map the vault entity to a VaultDto.
                var vaultDto = _mapper.Map<VaultDto>(vault);
                var vaultOperations = await _vaultOperationRepository.FindBy(x => x.VaultId==vault.Id).ToListAsync();
                vaultDto.VaultOperations=vaultOperations;
                // Step 4: Log successful retrieval of the vault.
                string successMessage = $"Vault retrieved successfully for BranchId: {request.BranchId}.";
                _logger.LogInformation(successMessage); // Log the success message.

                // Log the success using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultRetrieval,
                    LogLevelInfo.Information);

                // Step 5: Return a 200 OK response with the vault details.
                return ServiceResponse<VaultDto>.ReturnResultWith200(vaultDto, "Vault retrieved successfully.");
            }
            catch (Exception ex)
            {
                // Step 6: Handle and log unexpected errors.
                string errorMessage = $"An error occurred while retrieving the vault for BranchId: {request.BranchId}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage); // Log the exception details.

                // Log the error using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultRetrieval,
                    LogLevelInfo.Error);

                // Step 7: Return a 500 Internal Server Error response.
                return ServiceResponse<VaultDto>.Return500("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
