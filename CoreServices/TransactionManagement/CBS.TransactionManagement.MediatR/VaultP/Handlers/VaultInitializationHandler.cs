using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Repository.VaultP;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.MediatR.VaultP
{

    /// <summary>
    /// Handles the initialization of a vault with a specified amount and denominations.
    /// </summary>
    public class VaultInitializationHandler : IRequestHandler<VaultInitializationCommand, ServiceResponse<bool>>
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly ILogger<VaultInitializationHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;
        private readonly IAccountingDayRepository _accountingDayRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultInitializationHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">Repository for managing vault operations.</param>
        /// <param name="logger">Logger for logging activities and errors.</param>
        /// <param name="unitOfWork">Unit of work for managing transactions.</param>
        /// <param name="userInfoToken">User information for authentication and auditing.</param>
        public VaultInitializationHandler(
            IVaultRepository vaultRepository,
            ILogger<VaultInitializationHandler> logger,
            IUnitOfWork<TransactionContext> unitOfWork,
            UserInfoToken userInfoToken,
            IMediator mediator,
            IAccountingDayRepository accountingDayRepository)
        {
            _vaultRepository = vaultRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userInfoToken = userInfoToken;
            _mediator=mediator;
            _accountingDayRepository=accountingDayRepository;
        }

        /// <summary>
        /// Handles the vault initialization command by validating and depositing the specified amount into the vault.
        /// </summary>
        /// <param name="request">The command containing initialization details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A response indicating the success or failure of the vault initialization.</returns>
        public async Task<ServiceResponse<bool>> Handle(VaultInitializationCommand request, CancellationToken cancellationToken)
        {

            //request.AmountInHand=request.AmountInHand;
            // Generate a unique action reference for the transaction.
            string actionReference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"VIN-{_userInfoToken.BranchCode}-");
            var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
            try
            {
                // Step 1: Validate the total amount against the provided denominations.
                var (isValid, validationError) = CurrencyNotesMapper.ValidateAmountAndDenominations(request.TotalAmount, request.CurrencyNote);
                if (!isValid)
                {
                    // Log and audit validation failure.
                    string errorMessage = $"Vault initialization validation failed: {validationError}";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.VaultInitialization,
                        LogLevelInfo.Warning,
                        actionReference
                    );

                    // Return a 403 Forbidden response with the validation error message.
                    return ServiceResponse<bool>.Return403(validationError);
                }

                // Step 2: Retrieve the vault for the current branch.
                var vault = _vaultRepository.FindBy(x => x.BranchId == _userInfoToken.BranchID).FirstOrDefault();
                if (vault == null)
                {
                    // Log and audit if the vault is not found for the branch.
                    string errorMessage = $"Vault not found for branch {_userInfoToken.BranchName}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.VaultInitialization,
                        LogLevelInfo.Warning,
                        actionReference
                    );

                    // Return a 403 Forbidden response indicating the vault is missing.
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Step 3: Post accounting entries for the vault initialization.
                await AccountinPostingForVaultInitialization(actionReference, request.AmountInHand, request.CanProceed, accountingDay, request.AmountInVault);

                // Step 4: Perform the cash-in operation for the vault.
                _vaultRepository.CashInByDenomination(
                    request.TotalAmount, // Total amount to deposit into the vault.
                    request.CurrencyNote, // Denomination details for the cash.
                    _userInfoToken.BranchID, // Branch ID for the vault.
                    actionReference, // Unique reference for the transaction.
                    LogAction.VaultInitialization.ToString().ToUpper(), request.AmountInHand, request.AmountInVault, request.Note // Description of the transaction.

                );

                // Step 5: Log and audit the successful initialization.
                string successMessage = $"{vault.Name} initialized successfully with reference {actionReference}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultInitialization,
                    LogLevelInfo.Information,
                    actionReference
                );

                // Return a successful response indicating the vault has been initialized.
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors.

                // Step 6: Log and audit the exception.
                string errorMessage = $"An error occurred during vault initialization for branch {_userInfoToken.BranchName}. Error: {ex.Message}";
                _logger.LogError(errorMessage, ex);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultInitialization,
                    LogLevelInfo.Error,
                    actionReference
                );

                // Return a 500 Internal Server Error response with the error details.
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
        /// <summary>
        /// Posts accounting details for vault initialization.
        /// </summary>
        /// <param name="reference">Unique reference for the transaction.</param>
        /// <param name="cashInHand">Amount of cash in hand.</param>
        /// <param name="cashInVault">Amount of cash in the vault.</param>
        /// <returns>A service response indicating success or failure.</returns>
        private async Task<bool> AccountinPostingForVaultInitialization(string reference, decimal cashInHand, bool canProceed, DateTime accountingDay,decimal amountinVault)
        {
            string actionReference = reference;
            string narration = $"Vault Initialization for {_userInfoToken.BranchName} | Reference: {reference} | " +
                               $"Cash in Hand: {BaseUtilities.FormatCurrency(cashInHand)}";
            // Create command for vault initialization posting
            var vaultInitializationCommand = new CashInitializationCommand(cashInHand, reference, narration,canProceed,accountingDay, amountinVault);

            try
            {

                // Send command to mediator
                var response = await _mediator.Send(vaultInitializationCommand);

                // Handle failure case
                if (response.StatusCode != 200)
                {
                    var errorMessage = $"Accounting: Error: {response.Message}";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
                return response.Data.Status;
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                string errorMessage = $"An error occurred during Vault Initialization for {_userInfoToken.BranchName} | Reference: {reference}. Error: {ex.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
