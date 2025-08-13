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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddLoanAccountCommandHandler : IRequestHandler<AddLoanAccountCommand, ServiceResponse<bool>>
    {
        // Dependencies
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly IAccountRepository _AccountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddLoanAccountCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ISalaryExecutedRepository _salaryExtractRepository;
        // Constructor to initialize dependencies
        public AddLoanAccountCommandHandler(
            ISavingProductRepository savingProductRepository,
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddLoanAccountCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            ISalaryExecutedRepository salaryExtractRepository)
        {
            _savingProductRepository = savingProductRepository;
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _salaryExtractRepository=salaryExtractRepository;
        }

        // Method to handle the command
        public async Task<ServiceResponse<bool>> Handle(AddLoanAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.IsForSalaryTreatement)
                {
                    // Step 1: Get Salary Extracts
                    var salaryExtracts = await _salaryExtractRepository
                        .FindBy(x => x.FileUploadId == request.FileUploadId && !x.IsDeleted)
                        .ToListAsync();

                    // Step 2: Retrieve Loan Product outside of any transaction scope
                    var product = await GetLoanProduct();

                    // Step 3: Extract Customer IDs from Salary Extracts
                    var customerIds = salaryExtracts.Select(x => x.MemberReference).ToList();

                    // Step 4: Retrieve Customers from Account Repository Who DO NOT Have a Loan Account
                    // Step 1: Get customers who have a Loan account
                    var customersWithLoan = await _AccountRepository
                        .FindBy(x => customerIds.Contains(x.CustomerId) && x.AccountType == "Loan" && x.IsDeleted == false)
                        .Select(x => x.CustomerId)
                        .Distinct()
                        .ToListAsync();

                    // Step 2: Filter out customers who have a Loan account
                    var customersWithoutLoan = customerIds
                        .Where(id => !customersWithLoan.Contains(id))
                        .ToList();



                    // Step 5: Bulk Create Accounts for Eligible Customers
                    // Step 5: Create New Loan Accounts (Bulk Insert)
                    var newAccounts = customersWithoutLoan
                        .Select(customerId => CreateAccountEntity(request, product))
                        .ToList();

                    // Step 6: Bulk Insert in a Single Database Transaction

                    foreach (var account in newAccounts)
                    {
                        _AccountRepository.Add(account);
                    }
                    await _uow.SaveAsync();
                    await LogAndAudit($"Created {newAccounts.Count} loan accounts automatically", 200, request);
                    //if (newAccounts.Any())
                    //{
                    //    _AccountRepository.AddRange(newAccounts);
                    //    await _uow.SaveAsync();
                    //    await LogAndAudit($"Created {newAccounts.Count} loan accounts automatically", 200, request);
                    //}

                    return ServiceResponse<bool>.ReturnResultWith200(true, $"Successfully created {newAccounts.Count} loan accounts.");

                }
                else
                {
                    // Validate BankId and BranchId
                    if (!AreBankAndBranchIdsSet(request))
                        return ServiceResponse<bool>.Return409("Please set the BankId and BranchId fields");

                    // Retrieve Loan Product outside of any transaction scope
                    var product = await GetLoanProduct();
                    if (product == null)
                        return ServiceResponse<bool>.Return409("Loan ordinary account does not exist");

                    // Check if the customer already has a loan account
                    if (await CustomerHasAccountOfType(request.CustomerId, product.Id))
                        return ServiceResponse<bool>.ReturnSuccess("Your loan account was already set.");

                    // 🔹 **Create Account Entity**
                    var accountEntity = CreateAccountEntity(request, product);
                    //if (request.IsBGS)
                    //{
                    //    using (var scope = _serviceScopeFactory.CreateScope())
                    //    {
                    //        var scopedDbContext = scope.ServiceProvider.GetRequiredService<TransactionContext>();
                    //        var scopedAccountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                    //        var strategy = scopedDbContext.Database.CreateExecutionStrategy();
                    //        return await strategy.ExecuteAsync(async () =>
                    //        {
                    //            // 🚀 **Explicitly Suppress Transaction Scope to Avoid Conflict**
                    //            using (var suppressedScope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                    //            {
                    //                accountEntity.CreatedDate = DateTime.Now;
                    //                accountEntity.CreatedBy = request.UserInfoToken.Id;
                    //                accountEntity.ModifiedBy = request.UserInfoToken.Id;
                    //                accountEntity.ModifiedDate = new DateTime();
                    //                accountEntity.DeletedDate = new DateTime();
                    //                accountEntity.ModifiedDate = new DateTime();
                    //                scopedAccountRepository.Add(accountEntity);
                    //                await scopedDbContext.SaveChangesAsync(); // Save changes via EF directly

                    //                await LogAndAudit("Customer loan account created automatically successfully", 200, request);
                    //                suppressedScope.Complete(); // Ensure the transaction scope is completed
                    //                return ServiceResponse<bool>.ReturnResultWith200(true, "Customer loan account created automatically successfully");
                    //            }
                    //        });
                    //    }
                    //}

                    // 🔹 **Scenario 2: Normal Execution (Preserve TransactionScope)**
                    _AccountRepository.Add(accountEntity);
                    await _uow.SaveAsync();

                    await LogAndAudit("Customer loan account created automatically successfully", 200, request);
                    return ServiceResponse<bool>.ReturnResultWith200(true, "Customer loan account created automatically successfully");
                }
               

                // 🔹 **Scenario 1: BGS Execution (Suppress Transaction)**
               
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving Account: {e.Message}";
                await LogAndAudit(errorMessage, 500, request);
                return ServiceResponse<bool>.Return500(e);
            }
        }
        private Account CreateAccountEntity(AddLoanAccountCommand request, SavingProduct product)
        {
            var accountEntity = _mapper.Map<Account>(request);
            accountEntity.Id = BaseUtilities.GenerateUniqueNumber();
            accountEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            accountEntity.ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            accountEntity.AccountNumber = $"{product.AccountNuber}{request.CustomerId}";
            accountEntity.AccountType = product.AccountType;
            accountEntity.AccountName = $"{product.AccountType}";
            accountEntity.BranchCode = request.IsBGS ? request.UserInfoToken.BranchCode : _userInfoToken.BranchCode;
            accountEntity.CustomerName = request.CustomerId;
            accountEntity.BranchId = request.IsBGS ? request.UserInfoToken.BranchID : _userInfoToken.BranchID;
            accountEntity.ProductId = product.Id;

            return accountEntity;
        }

        // Check if bankId and BranchId are set
        private bool AreBankAndBranchIdsSet(AddLoanAccountCommand request)
        {
            return request.BankId != null && request.BranchId != null;
        }

        // Get the loan product
        private async Task<SavingProduct> GetLoanProduct()
        {
            return await _savingProductRepository.FindBy(x => x.AccountType.ToLower() == "loan").FirstOrDefaultAsync();
        }

        // Check if customer already has a loan account
        private async Task<bool> CustomerHasAccountOfType(string customerId, string productId)
        {
            var existingAccount = await _AccountRepository.FindBy(c => c.ProductId == productId && c.CustomerId == customerId)
                .Include(a => a.Product)
                .FirstOrDefaultAsync();

            return existingAccount != null;
        }

        // Log and audit the action
        private async Task LogAndAudit(string message, int statusCode, AddLoanAccountCommand command)
        {
            _logger.LogError(message);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), command, message, LogLevelInfo.Error.ToString(), statusCode, _userInfoToken.Token);
        }
    }

}
