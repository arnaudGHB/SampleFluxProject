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
    /// Handles retrieval of a vault by its unique Id.
    /// </summary>
    public class GetVaultByIdHandler : IRequestHandler<GetVaultByIdQuery, ServiceResponse<VaultDto>>
    {
        private readonly IVaultRepository _vaultRepository; // Repository for managing vaults.
        private readonly ILogger<GetVaultByIdHandler> _logger; // Logger for recording events and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IVaultOperationRepository _vaultOperationRepository; // Repository for managing vaults.

        /// <summary>
        /// Initializes a new instance of the <see cref="GetVaultByIdHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The repository for managing vaults.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public GetVaultByIdHandler(IVaultRepository vaultRepository, ILogger<GetVaultByIdHandler> logger, IMapper mapper, IVaultOperationRepository vaultOperationRepository = null)
        {
            _vaultRepository = vaultRepository; // Inject the vault repository.
            _logger = logger; // Inject the logger for logging.
            _mapper = mapper; // Inject AutoMapper for mapping objects.
            _vaultOperationRepository=vaultOperationRepository;
        }

        /// <summary>
        /// Handles the query to get a vault by Id.
        /// </summary>
        /// <param name="request">The query containing the vault Id.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A response containing the vault details.</returns>
        public async Task<ServiceResponse<VaultDto>> Handle(GetVaultByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate the input parameters.
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    const string error = "Vault Id is a required field.";
                    _logger.LogError(error); // Log the error.

                    // Log and audit using Utility Logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultRetrieval,
                        LogLevelInfo.Warning);

                    return ServiceResponse<VaultDto>.Return400(error); // Return 400 Bad Request.
                }

                // Step 2: Retrieve the vault by Id from the repository.
                var vault = await _vaultRepository.FindAsync(request.Id);

                // Step 3: Check if the vault was found.
                if (vault == null)
                {
                    const string error = "No vault found for the specified Id.";
                    _logger.LogError(error); // Log the error.

                    // Log and audit using Utility Logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.VaultRetrieval,
                        LogLevelInfo.Warning);

                    return ServiceResponse<VaultDto>.Return404(error); // Return 404 Not Found.
                }

                // Step 4: Map the vault entity to a VaultDto object.
                var vaultDto = _mapper.Map<VaultDto>(vault);
                var vaultOperations = await _vaultOperationRepository.FindBy(x => x.VaultId==vault.Id).ToListAsync();
                vaultDto.VaultOperations=vaultOperations;

                // Step 5: Log successful retrieval of the vault.
                string successMessage = $"Vault with Id: {request.Id} retrieved successfully.";
                _logger.LogInformation(successMessage); // Log the success.

                // Log and audit using Utility Logger.
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultRetrieval,
                    LogLevelInfo.Information);

                // Step 6: Return the VaultDto in the response.
                return ServiceResponse<VaultDto>.ReturnResultWith200(vaultDto, "Vault retrieved successfully.");
            }
            catch (Exception ex)
            {
                // Step 7: Handle unexpected exceptions and log the error.
                string errorMessage = $"An unexpected error occurred while retrieving the vault for Id: {request.Id}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage); // Log the exception details.

                // Log and audit using Utility Logger.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultRetrieval,
                    LogLevelInfo.Error);

                // Step 8: Return a 500 Internal Server Error response.
                return ServiceResponse<VaultDto>.Return500("An unexpected error occurred. Please try again later.");
            }
        }
    }


}
