using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.VaultP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.VaultP
{


    /// <summary>
    /// Handles the update of an existing vault.
    /// </summary>
    public class UpdateVaultHandler : IRequestHandler<UpdateVaultCommand, ServiceResponse<bool>>
    {
        private readonly IVaultRepository _vaultRepository; // Repository for managing vault data.
        private readonly ILogger<UpdateVaultHandler> _logger; // Logger for logging events and errors.
        private readonly IMapper _mapper; // AutoMapper instance for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for transaction management.

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateVaultHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The repository for managing vaults.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateVaultHandler(
            IVaultRepository vaultRepository,
            ILogger<UpdateVaultHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow)
        {
            _vaultRepository = vaultRepository; // Inject the vault repository.
            _logger = logger; // Inject the logger for event and error logging.
            _mapper = mapper; // Inject the AutoMapper instance for mapping objects.
            _uow = uow; // Inject the Unit of Work for managing database transactions.
        }

        /// <summary>
        /// Handles the UpdateVaultCommand to update an existing vault.
        /// </summary>
        /// <param name="request">The command containing updated vault details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A response indicating the success or failure of the vault update.</returns>
        public async Task<ServiceResponse<bool>> Handle(UpdateVaultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate the input parameters.
                if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.BranchId))
                {
                    const string error = "Vault Id, Name, and BranchId are required fields."; // Error message for missing fields.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultUpdate,
                        LogLevelInfo.Error);

                    return ServiceResponse<bool>.Return400(error); // Return a 400 Bad Request response.
                }

                // Step 2: Check if the specified vault exists.
                var existingVault = await _vaultRepository.FindAsync(request.Id); // Fetch the vault by its ID.
                if (existingVault == null) // If the vault is not found...
                {
                    const string error = "The specified vault does not exist or has been deleted."; // Error message for missing vault.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.VaultUpdate,
                        LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return404(error); // Return a 404 Not Found response.
                }

                // Step 3: Log the initiation of the vault update.
                string initiationMessage = $"Updating vault with Id: {request.Id}, Name: {request.Name}, Branch code: {request.BranchCode}.";
                _logger.LogInformation(initiationMessage); // Log the initiation message.
                // Step 4: Map the updated properties from the request to the existing vault entity.
                _mapper.Map(request, existingVault); // Use AutoMapper to update the entity.

                // Step 5: Save the updated vault entity to the repository.
                _vaultRepository.Update(existingVault); // Mark the vault for update.
                await _uow.SaveAsync(); // Commit the transaction.

                // Step 6: Log the successful update.
                string successMessage = $"vault with Id: {request.Id}, Name: {request.Name}, Branch code: {request.BranchCode} updated successfully.";
                _logger.LogInformation(successMessage); // Log the success.

                // Log the success using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultUpdate,
                    LogLevelInfo.Information);

                // Step 7: Return a success response with a 200 OK status.
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Step 8: Log unexpected errors during the update process.
                string errorMessage = $"An error occurred while updating the vault with Id: {request.Id}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage); // Log the exception and vault ID.

                // Log the error using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultUpdate,
                    LogLevelInfo.Error);

                // Step 9: Return a 500 Internal Server Error response.
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }
}
