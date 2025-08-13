using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.Repository.VaultP;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.MediatR.VaultP
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using CBS.TransactionManagement.Common.UnitOfWork;
    using CBS.TransactionManagement.Data.Entity.CashVaultP;
    using CBS.TransactionManagement.Domain;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Handles the creation of a new vault.
    /// </summary>
    public class AddVaultHandler : IRequestHandler<AddVaultCommand, ServiceResponse<bool>>
    {
        private readonly IVaultRepository _vaultRepository; // Repository for vault operations.
        private readonly ILogger<AddVaultHandler> _logger; // Logger for logging events and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for database transactions.

        /// <summary>
        /// Initializes a new instance of the <see cref="AddVaultHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The repository for managing vaults.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public AddVaultHandler(
            IVaultRepository vaultRepository,
            ILogger<AddVaultHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _vaultRepository = vaultRepository; // Inject the vault repository.
            _logger = logger; // Inject the logger for event and error logging.
            _mapper = mapper; // Inject AutoMapper for object mapping.
            _uow = uow; // Inject Unit of Work for database operations.
        }

        /// <summary>
        /// Handles the AddVaultCommand to create a new vault.
        /// </summary>
        /// <param name="request">The command containing vault details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A response indicating the success or failure of the vault creation.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddVaultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate input parameters.
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.BranchId))
                {
                    const string error = "Vault Name and BranchId are required fields."; // Validation error message.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultCreation,
                        LogLevelInfo.Error);

                    return ServiceResponse<bool>.Return400(error); // Return 400 Bad Request response.
                }

                // Step 2: Check if a vault with the same name already exists in the branch.
                var existingVault = await _vaultRepository
                    .FindBy(v => v.Name == request.Name && v.BranchId == request.BranchId && !v.IsDeleted)
                    .FirstOrDefaultAsync();

                if (existingVault != null)
                {
                    const string error = "A vault with the same name already exists in the specified branch."; // Error message for duplicate vault.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.Conflict,
                        LogAction.VaultCreation,
                        LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return400(error); // Return 400 Bad Request response.
                }

                // Step 3: Check if more than one vault already exists in the branch.
                var vaultCountInBranch = _vaultRepository
                    .FindBy(v => v.BranchId == request.BranchId && !v.IsDeleted)
                    .Count();

                if (vaultCountInBranch >= 1)
                {
                    const string error = "Only one vault is allowed per branch."; // Error message for branch limit.
                    _logger.LogError(error); // Log the error.

                    // Log the error using utility logger.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.Conflict,
                        LogAction.VaultCreation,
                        LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return400(error); // Return 400 Bad Request response.
                }

                // Step 4: Log the initiation of the vault creation.
                string initiationMessage = $"Creating a new vault with Name: {request.Name}, BranchId: {request.BranchId}.";
                _logger.LogInformation(initiationMessage); // Log the initiation message.

                // Log the initiation using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    initiationMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultCreation,
                    LogLevelInfo.Information);

                // Step 5: Map the AddVaultCommand to a Vault entity using AutoMapper.
                var vault = _mapper.Map<Vault>(request); // Map request to entity.
                vault.Id = BaseUtilities.GenerateUniqueNumber(); // Generate a unique ID for the vault.
                vault.CurrentBalance = 0; // Initialize current balance to zero.
                vault.PreviouseBalance = 0; // Initialize previous balance to zero.
                vault.LastOperation= "Creation";

        // Step 6: Save the vault to the repository.
        _vaultRepository.Add(vault); // Add the vault to the repository.
                await _uow.SaveAsync(); // Commit the changes.

                // Step 7: Log the successful creation.
                string successMessage = $"Vault with Name: {request.Name} and BranchId: {request.BranchId} created successfully.";
                _logger.LogInformation(successMessage); // Log the success message.

                // Log the success using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.Created,
                    LogAction.VaultCreation,
                    LogLevelInfo.Information);

                // Step 8: Return a 200 OK response with the success message.
                return ServiceResponse<bool>.ReturnResultWith200(true, "Vault created successfully.");
            }
            catch (Exception ex)
            {
                // Step 9: Handle and log unexpected errors during vault creation.
                string errorMessage = $"An error occurred while creating the vault with Name: {request.Name}, BranchId: {request.BranchId}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage); // Log the exception and request details.

                // Log the error using utility logger.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultCreation,
                    LogLevelInfo.Error);

                // Step 10: Return a 500 Internal Server Error response.
                return ServiceResponse<bool>.Return500("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
