using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using Newtonsoft.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.Data.Enum;

namespace CBS.AccountManagement.MediatR.Handlers
{
  
    public class UpdateProductAccountingBookCommandHandler : IRequestHandler<UpdateProductAccountingBookCommand, ServiceResponse<UpdateNewProductAccountingBook>>
    {
 
        private readonly ILogger<UpdateProductAccountingBookCommandHandler> _logger; 
        private readonly IMapper _mapper;  
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IProductAccountingBookRepository _productAccountingBookRepository; 
        private readonly IChartOfAccountManagementPositionRepository _chartofaccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IAccountRepository _accountRepository;

        public UpdateProductAccountingBookCommandHandler(
            UserInfoToken userInfoToken,
            IAccountingRuleEntryRepository AccountTypeRepository,
              
            ILogger<UpdateProductAccountingBookCommandHandler> logger,
            IMapper mapper,
             IProductAccountingBookRepository productAccountingBookRepository,
            IAccountRepository accountRepository,
                     IChartOfAccountManagementPositionRepository chartofaccountRepository,
            IUnitOfWork<POSContext> uow = null)
        {
            _accountingRuleEntryRepository = AccountTypeRepository;
            _productAccountingBookRepository = productAccountingBookRepository;
            _chartofaccountRepository = chartofaccountRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _accountRepository =accountRepository;
        }


        public async Task<ServiceResponse<UpdateNewProductAccountingBook>> Handle(UpdateProductAccountingBookCommand request, CancellationToken cancellationToken)
        {
            UpdateNewProductAccountingBook updateProductAccountingBookCommand = new UpdateNewProductAccountingBook();
            updateProductAccountingBookCommand.ItemsToCreate = new List<ProductAccountBookDetail>();
            updateProductAccountingBookCommand.WasCompletelySuccessFull = true;
            try
            {
                // Retrieve the AccountType entity to be updated from the repository

                string errorMessage = " ";

                var existingProductAccountingBooks = _productAccountingBookRepository.FindBy(x => x.Name == request.ProductName);

                if (!existingProductAccountingBooks.Any())
                {
                    errorMessage = $"There is no product that exist in the system with as {request.ProductName} .";

                    LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 404);
                    return ServiceResponse<UpdateNewProductAccountingBook>.Return404(errorMessage);
                }
                List<AccountingRuleEntry> updatedEntries = new List<AccountingRuleEntry>();
                List<ProductAccountingBook> updatedProductAccountingBooks = new List<ProductAccountingBook>();
                foreach (var item in request.ProductAccountBookDetails)
                {
                    string Id = request.ProductId + "@" + item.Name;
                 
                    var AccountingEntryRule = _accountingRuleEntryRepository.All.Where(x => x.EventCode==Id);

                    if (AccountingEntryRule.Any())
                    {
                        var model = AccountingEntryRule.FirstOrDefault();
                        string OldChartOfAccountId = model.DeterminationAccountId;
                        if (OldChartOfAccountId == item.ChartOfAccountId)
                            continue;
                        model.DeterminationAccountId = item.ChartOfAccountId;
                        var entryModel = existingProductAccountingBooks.ToList().Find(x => x.Id == Id);
                        entryModel.ChartOfAccountId = item.ChartOfAccountId;
                        entryModel.ChartOfAccountManagementPositionId = item.ChartOfAccountId;
                        var NewAccountInUsed = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == item.ChartOfAccountId);
                        var modelNewAccountInUsed = NewAccountInUsed.Any() ? NewAccountInUsed.FirstOrDefault() : null;
                        var OldAccountInUsed = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == OldChartOfAccountId);
                        var modelOldAccountInUsed = OldAccountInUsed.Any() ? OldAccountInUsed.FirstOrDefault() : null;

                        if ((modelOldAccountInUsed != null) && (modelNewAccountInUsed != null))
                        {
                            if (modelOldAccountInUsed.Id == modelNewAccountInUsed.Id)
                            {
                                updateProductAccountingBookCommand.WasCompletelySuccessFull = false;
                                modelOldAccountInUsed.ChartOfAccountManagementPosition = await _chartofaccountRepository.FindAsync(OldChartOfAccountId);
                                errorMessage += $"The Old account number :{modelOldAccountInUsed.ChartOfAccountManagementPosition.AccountNumber}{modelOldAccountInUsed.ChartOfAccountManagementPosition.PositionNumber.PadRight(3, '0')}";
                                modelNewAccountInUsed.ChartOfAccountManagementPosition = await _chartofaccountRepository.FindAsync(item.ChartOfAccountId);
                                errorMessage += $"The New account number :{modelOldAccountInUsed.ChartOfAccountManagementPosition.AccountNumber}{modelOldAccountInUsed.ChartOfAccountManagementPosition.PositionNumber.PadRight(3, '0')}";
                                continue;
                            }
                             
                     
                        }
                        else
                        {
                            if ((modelNewAccountInUsed != null))
                            {
                                updateProductAccountingBookCommand.WasCompletelySuccessFull = false;
                                modelNewAccountInUsed.ChartOfAccountManagementPosition = await _chartofaccountRepository.FindAsync(item.ChartOfAccountId);
                                errorMessage += $"The New account number :{modelNewAccountInUsed.ChartOfAccountManagementPosition.AccountNumber}{modelNewAccountInUsed.ChartOfAccountManagementPosition.PositionNumber.PadRight(3, '0')}";


                            }
                            if ((modelOldAccountInUsed != null))
                            {
                                updateProductAccountingBookCommand.WasCompletelySuccessFull = false;
                                modelOldAccountInUsed.ChartOfAccountManagementPosition = await _chartofaccountRepository.FindAsync(OldChartOfAccountId);
                                errorMessage += $"The Old account number :{modelOldAccountInUsed.ChartOfAccountManagementPosition.AccountNumber}{modelOldAccountInUsed.ChartOfAccountManagementPosition.PositionNumber.PadRight(3, '0')}";
                                 
                                continue;
                            }
                           
                          
                           
                        }

                        updatedEntries.Add(model);
                        updatedProductAccountingBooks.Add(entryModel);

                    }
                    else
                    {
                        var product = await _productAccountingBookRepository.FindAsync(Id);
                        updateProductAccountingBookCommand.ItemsToCreate.Add(item);
                        updateProductAccountingBookCommand.hasNewAccountingBooks = true;

                    }


                }
                _productAccountingBookRepository.UpdateRangeInCaseCade(updatedProductAccountingBooks);
                _accountingRuleEntryRepository.UpdateRangeInCaseCade(updatedEntries);
       

               //await _uow.SaveAsync();
                LogAuditSuccess(request);

                return ServiceResponse<UpdateNewProductAccountingBook>.ReturnResultWith200(updateProductAccountingBookCommand, updateProductAccountingBookCommand.WasCompletelySuccessFull ? "Update was successfully done." : errorMessage);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 500);
                return ServiceResponse<UpdateNewProductAccountingBook>.Return500(e);
            }

        }



        private void LogAndAuditError(UpdateProductAccountingBookCommand request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(),
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode, _userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(UpdateProductAccountingBookCommand request)
        {
            string successMessage = "AccountType update successfully.";
            APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateProductAccountingBookCommand",
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }


        //public async Task<ServiceResponse<UpdateNewProductAccountingBook>> Handle(UpdateProductAccountingBookCommand request, CancellationToken cancellationToken)
        //{
        //    // Initialize the result object with default values
        //    var updateResult = new UpdateNewProductAccountingBook
        //    {
        //        ItemsToCreate = new List<ProductAccountBookDetail>(),
        //        WasCompletelySuccessFull = true
        //    };
           
        //    try
        //    {
        //        // Step 1: Validate if the product exists
        //        if (!ValidateProduct(request.ProductName, out var errorMessage))
        //        {
        //            // Log and return a 404 response if the product doesn't exist
        //            LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 404);
        //            return ServiceResponse<UpdateNewProductAccountingBook>.Return404(errorMessage);
        //        }

        //        // Collections to hold updated entries and books for bulk update
        //        var updatedEntries = new List<AccountingRuleEntry>();
        //        var updatedBooks = new List<ProductAccountingBook>();
        //        var notUpdateAccount = new List<AccountNotUpdated>();
        //        // Step 2: Process each product detail in the request
        //        foreach (var detail in request.ProductAccountBookDetails)
        //        {
        //            await ProcessDetailAsync(notUpdateAccount,detail, updatedEntries, updatedBooks, updateResult, request);
        //        }
           
        //        // Step 3: Perform batch updates for entries and books
        //        _accountingRuleEntryRepository.UpdateRange(updatedEntries);
        //        _productAccountingBookRepository.UpdateRange(updatedBooks);

        //        // Commit the changes to the database       await _uow.SaveAsync();
        //        await _uow.SaveAsync();

        //        // Step 4: Log success and return response
        //        LogAuditSuccess(request);
        //        var responseMessage = updateResult.WasCompletelySuccessFull
        //            ? "Update was successfully done."
        //            : updateResult.ErrorMessage;
        //        updateResult.ItemNotUpdated = notUpdateAccount;
        //        return ServiceResponse<UpdateNewProductAccountingBook>.ReturnResultWith200(updateResult, responseMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected exceptions, log the error, and return a 500 response
        //        var errorMessage = $"Error occurred while updating AccountType: {ex.Message}";
        //        LogAndAuditError(request, errorMessage, LogLevelInfo.Error, 500);
        //        return ServiceResponse<UpdateNewProductAccountingBook>.Return500(ex);
        //    }
        //}


        /// <summary>
        /// Validates if the product exists in the database.
        /// </summary>
        /// <param name="productName">The name of the product to validate.</param>
        /// <param name="errorMessage">The error message if the product is not found.</param>
        /// <returns>True if the product exists; otherwise, false.</returns>
        private bool ValidateProduct(string productName, out string errorMessage)
        {
            var existingProductAccountingBooks = _productAccountingBookRepository.FindBy(x => x.Name == productName);
            if (!existingProductAccountingBooks.Any())
            {
                errorMessage = $"There is no product that exists in the system with the name {productName}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
        /// <summary>
        /// Processes a single product detail, updating or adding it as needed.
        /// </summary>
        /// <param name="detail">The product accounting book detail to process.</param>
        /// <param name="updatedEntries">List to store updated accounting rule entries.</param>
        /// <param name="updatedBooks">List to store updated product accounting books.</param>
        /// <param name="updateResult">The result object to track processing success.</param>
        /// <param name="request">The original request object for logging.</param>
        private async Task  ProcessDetailAsync(
            List<AccountNotUpdated> notUpdateAccountd,
            ProductAccountBookDetail detail,
            List<AccountingRuleEntry> updatedEntries,
            List<ProductAccountingBook> updatedBooks,
            UpdateNewProductAccountingBook updateResult,
            UpdateProductAccountingBookCommand request)
        {
            // Construct a unique ID for the accounting book entry
            string id = request.ProductId + "@" + detail.Name;
            var accountingEntryRule = _accountingRuleEntryRepository.FindBy(x => x.ProductAccountingBookId.Equals(id));

            if (accountingEntryRule.Any())
            {
                // Update existing accounting rule entry if found
                var model = accountingEntryRule.FirstOrDefault();
                string oldChartOfAccountId = model.DeterminationAccountId;

                // Skip if the account ID hasn't changed
                if (oldChartOfAccountId == detail.ChartOfAccountId)
                {
                    notUpdateAccountd.Add(new AccountNotUpdated
                    {
                        AccountId = detail.ChartOfAccountId,
                        AccountName = (await _chartofaccountRepository.FindAsync(detail.ChartOfAccountId)).Description

                    });
                    return;
                }
                 
                // Update model and related properties
                model.DeterminationAccountId = detail.ChartOfAccountId;
                var entryModel = _productAccountingBookRepository.FindBy(x => x.Id == id).FirstOrDefault();
                entryModel.ChartOfAccountId = detail.ChartOfAccountId;

                // Handle account conflict scenarios
                await HandleAccountConflicts(detail, oldChartOfAccountId, updateResult);

                // Add updated models to the respective lists
                updatedEntries.Add(model);
                updatedBooks.Add(entryModel);
              
            }
            else
            {
                // Add new product accounting book detail if it doesn't exist
                updateResult.ItemsToCreate.Add(detail);
                updateResult.hasNewAccountingBooks = true;
                
            }
        }
        /// <summary>
        /// Checks and handles conflicts between old and new accounts.
        /// </summary>
        /// <param name="detail">The product detail being processed.</param>
        /// <param name="oldChartOfAccountId">The old chart of account ID.</param>
        /// <param name="updateResult">The result object to track success and errors.</param>
        private async Task HandleAccountConflicts(
            ProductAccountBookDetail detail,
            string oldChartOfAccountId,
            UpdateNewProductAccountingBook updateResult)
        {
            var newAccountInUse = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == detail.ChartOfAccountId).FirstOrDefault();
            var oldAccountInUse = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == oldChartOfAccountId).FirstOrDefault();

            if (newAccountInUse != null && oldAccountInUse != null && newAccountInUse != oldAccountInUse)
            {
                updateResult.WasCompletelySuccessFull = false;
                oldAccountInUse.ChartOfAccountManagementPosition = await _chartofaccountRepository.FindAsync(oldChartOfAccountId);
                newAccountInUse.ChartOfAccountManagementPosition = await _chartofaccountRepository.FindAsync(detail.ChartOfAccountId);

                updateResult.ErrorMessage += $"Conflict detected between accounts. " +
                    $"Old Account: {oldAccountInUse.ChartOfAccountManagementPosition.AccountNumber} " +
                    $"New Account: {newAccountInUse.ChartOfAccountManagementPosition.AccountNumber}\n";
            }
        }

    }
}