using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Command;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddDefaultAccountCommandHandler : IRequestHandler<AddDefaultAccountCommand, ServiceResponse<bool>>
    {
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        public IMediator _mediator { get; set; } // Mediator for handling requests.

        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDefaultAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddDefaultAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDefaultAccountCommandHandler(
            ISavingProductRepository savingProductRepository,
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddDefaultAccountCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _savingProductRepository = savingProductRepository;
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = UserInfoToken;
            _uow = uow;
            _mediator = mediator;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        /// <summary>
        /// Handles the automatic creation of default accounts for a new member.
        /// This process assigns all active saving products (configured for auto-addition) 
        /// to the specified customer if they don't already have an account of that type.
        /// </summary>
        /// <param name="request">The command containing customer details.</param>
        /// <param name="cancellationToken">A cancellation token to stop execution if needed.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddDefaultAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all active saving products that should be automatically assigned to members.
                var ordinaryAccounts = await _savingProductRepository
                    .FindBy(x => x.AutoAddToMember && x.ActiveStatus && !x.IsDeleted)
                    .ToListAsync();

                if (!ordinaryAccounts.Any())
                {
                    string noProductMessage = $"No active saving products are available for automatic assignment to new members.";
                    _logger.LogInformation(noProductMessage);
                    await BaseUtilities.LogAndAuditAsync(noProductMessage, request, HttpStatusCodeEnum.AutoAccountCreation, LogAction.Create, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(noProductMessage);
                }

                // Step 2: Store created account details for logging
                var createdAccountsList = new List<string>();

                // Step 3: Iterate over each saving product and assign an account to the customer if needed.
                foreach (var a in ordinaryAccounts)
                {
                    // Step 3.1: Check if the customer already has an account for this saving product.
                    var existingAccount = await _AccountRepository
                        .FindBy(c => c.ProductId == a.Id && c.CustomerId == request.CustomerId)
                        .Include(a => a.Product)
                        .FirstOrDefaultAsync();

                    if (existingAccount != null)
                    {
                        // Step 3.2: Log and skip account creation if the account already exists.
                        string logMessage = $"[INFO] Skipping account [Name: {a.Name}, Type: {a.AccountType}] creation for Member: [Reference Number {request.CustomerId}, Name: {request.CustomerName}]. Account already exists.";
                        _logger.LogInformation(logMessage);
                        await BaseUtilities.LogAndAuditAsync(logMessage, request, HttpStatusCodeEnum.OK, LogAction.SkipAutoAccountCreation, LogLevelInfo.Information);
                        continue; // Skip to the next product.
                    }

                    // Step 4: Generate an account number for the new account.
                    string accountNumber = a.AccountNuber ?? $"{a.Code}";

                    // Step 5: Create a new account entity with the necessary details.
                    var AccountEntity = new Account
                    {
                        CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Convert UTC to local time.
                        ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                        Id = BaseUtilities.GenerateUniqueNumber(), // Generate a unique ID.
                        AccountNumber = $"{accountNumber}{request.CustomerId}", // Construct the account number.
                        AccountType = a.AccountType,
                        AccountName = $"{a.Name} - {a.AccountType}", // Improved name format.
                        BranchCode = _userInfoToken.BranchCode, // Assign the branch code.
                        CustomerName = request.CustomerName, // Assign customer name.
                        BranchId = _userInfoToken.BranchID, // Assign branch ID.
                        BankId = _userInfoToken.BankID, // Assign bank ID.
                        ProductId = a.Id, // Link to the saving product.
                        CustomerId = request.CustomerId // Assign the customer ID.
                    };

                    // Step 6: Add the newly created account to the repository.
                    _AccountRepository.Add(AccountEntity);

                    // Step 6.1: Store the account details for logging
                    string accountCreatedMessage = $"[INFO] New account created: [Name: {a.Name}, Type: {a.AccountType}, Account Number: {AccountEntity.AccountNumber}] for Member: [Reference Number {request.CustomerId}, Name: {request.CustomerName}].";
                    createdAccountsList.Add(accountCreatedMessage);

                    // Step 6.2: Log account creation success.
                    _logger.LogInformation(accountCreatedMessage);
                    await BaseUtilities.LogAndAuditAsync(accountCreatedMessage, request, HttpStatusCodeEnum.OK, LogAction.AutoAccountCreation, LogLevelInfo.Information);
                }

                // Step 7: Retrieve branch details for audit and reporting purposes.
                var TellerBranch = await GetBranch(_userInfoToken.BranchID);

                // Step 8: Log this operation as a new member registration in the daily dashboard.
                var cashOperation = new CashOperation(
                    _userInfoToken.BranchID,
                    0, // No cash amount for this operation.
                    0,
                    TellerBranch.name,
                    TellerBranch.branchCode,
                    CashOperationType.NewMember,
                    LogAction.MemberRegistration,
                    null
                );

                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Step 9: Save all pending changes to the database.
                await _uow.SaveAsync();

                // Step 10: Compile final success message with all created accounts.
                string createdAccountsSummary = createdAccountsList.Any()
                    ? string.Join("\n", createdAccountsList)
                    : "No new accounts were created. All accounts already existed.";

                string successMessage = $"[SUCCESS] Default accounts successfully created for Member: [Reference Number {request.CustomerId}, Name: {request.CustomerName}].\nCreated Accounts:\n{createdAccountsSummary}";

                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AutoAccountCreation, LogLevelInfo.Information);

                // Step 11: Return success response.
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception e)
            {
                // Step 12: Log and audit the error.
                string errorMessage = $"[ERROR] Account creation failed for CustomerId {request.CustomerId}. Reason: {e.Message}";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AutoAccountCreation, LogLevelInfo.Error);

                // Step 13: Return a failure response.
                return ServiceResponse<bool>.Return500(e);
            }
        }
        private async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
    }

}
