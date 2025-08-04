using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Data.Entity.OtherCashInP;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class CurrencyNotesDto
    {
        public string Id { get; set; }
        public string DinominationType { get; set; }
        public string Denomination { get; set; }
        public int Value { get; set; }
        public int BalanceInValue { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string ReferenceId { get; set; }
    }

    public static class CurrencyNotesMapper
    {
        // Function to check if today is a holiday
        public static bool IsTodayHoliday(Config config)
        {
            // Get today's day of the week
            DayOfWeek today = DateTime.Today.DayOfWeek;
            // Check if today is a holiday based on the configuration
            switch (today)
            {
                case DayOfWeek.Monday:
                    return config.MondayIsHoliday;
                case DayOfWeek.Tuesday:
                    return config.TuesDayIsHoliday;
                case DayOfWeek.Wednesday:
                    return config.MWednessdayIsHoliday;
                case DayOfWeek.Thursday:
                    return config.ThursdayIsHoliday;
                case DayOfWeek.Friday:
                    return config.FridayIsHoliday;
                case DayOfWeek.Saturday:
                    return config.SaturdayIsHoliday;
                case DayOfWeek.Sunday:
                    return config.SundayIsHoliday;
                default:
                    return false;
            }
        }
        public static TransferThirdPartyDto MapTransactionToTransferThirdParty(TransactionDto transaction)
        {
            TransferThirdPartyDto transferThirdParty = new TransferThirdPartyDto();

            // Map properties from TransactionDto to TransferThirdPartyDto
            transferThirdParty.Id = transaction.Id;
            transferThirdParty.Amount = transaction.Amount;
            transferThirdParty.CustomerId = transaction.CustomerId;
            transferThirdParty.SenderAccountNumber = transaction.AccountNumber; // Assuming this is the sender's account number
            transferThirdParty.ReciverAccountNumber = transaction.ReceiverAccountId; // Assuming this is the receiver's account number
            transferThirdParty.Status = transaction.Status;
            transferThirdParty.TransactionReference = transaction.TransactionReference;
            transferThirdParty.Note = transaction.Note;
            transferThirdParty.Fee = transaction.Fee;
            transferThirdParty.TransactionDate = transaction.CreatedDate;
            transferThirdParty.AmountInWord = transaction.AmountInWord;
            transferThirdParty.BranchCode = transaction.BranchId; // Assuming this is the branch code
            transferThirdParty.TellerCode = transaction.TellerId; // Assuming this is the teller code
            transferThirdParty.UserName = transaction.CreatedBy;
            transferThirdParty.ExternalReference = transaction.ExternalReference;
            transferThirdParty.ExternalApplicationName = transaction.ExternalApplicationName;

            return transferThirdParty;
        }
        /// <summary>
        /// Validates the total amount and the denominations provided in the request.
        /// </summary>
        /// <param name="amount">The total amount expected to match the sum of denominations.</param>
        /// <param name="denominations">The denominations provided for validation.</param>
        /// <returns>A tuple containing a boolean indicating success and an error message if validation fails.</returns>
        public static (bool IsValid, string ErrorMessage) ValidateAmountAndDenominations(decimal amount, CurrencyNotesRequest denominations)
        {
            // Calculate the total amount from the denominations.
            decimal calculatedTotal =
                (denominations.Note10000 * 10000) +
                (denominations.Note5000 * 5000) +
                (denominations.Note2000 * 2000) +
                (denominations.Note1000 * 1000) +
                (denominations.Note500 * 500) +
                (denominations.Coin500 * 500) +
                (denominations.Coin100 * 100) +
                (denominations.Coin50 * 50) +
                (denominations.Coin25 * 25) +
                (denominations.Coin10 * 10) +
                (denominations.Coin5 * 5) +
                (denominations.Coin1 * 1);

            // Validate if the calculated total matches the provided amount.
            if (calculatedTotal != amount)
            {
                string errorMessage = $"Validation failed. The calculated total from denominations ({BaseUtilities.FormatCurrency(calculatedTotal)}) does not match the provided amount ({BaseUtilities.FormatCurrency(amount)}).";
                return (false, errorMessage);
            }

            // If validation passes, return true.
            return (true, string.Empty);
        }
        /// <summary>
        /// Calculates the total amount based on the denominations provided.
        /// </summary>
        /// <param name="notes">The denominations and their counts.</param>
        /// <returns>The total calculated amount.</returns>
        public static decimal CalculateTotalAmount(CurrencyNotesRequest notes)
        {
            return (notes.Note10000 * 10000) +
                   (notes.Note5000 * 5000) +
                   (notes.Note2000 * 2000) +
                   (notes.Note1000 * 1000) +
                   (notes.Note500 * 500) +
                   (notes.Coin500 * 500) +
                   (notes.Coin100 * 100) +
                   (notes.Coin50 * 50) +
                   (notes.Coin25 * 25) +
                   (notes.Coin10 * 10) +
                   (notes.Coin5 * 5) +
                   (notes.Coin1 * 1);
        }

        public static List<SavingProductFee> GetSavingProductFee(List<SavingProductFee> savingProductFees, string operationType, bool isHoliday)
        {
            // Filter SavingProductFee based on operation type
            var feesForOperation = savingProductFees.Where(fee => fee.FeeType.Equals(operationType, StringComparison.OrdinalIgnoreCase));

            // If it's a holiday, filter out fees that don't apply on holidays
            if (isHoliday)
                feesForOperation = feesForOperation.Where(fee => fee.Fee.IsAppliesOnHoliday);

            // Get the list applicable fees
            return feesForOperation.ToList();
        }
        public static SavingProductFee GetFeeByFeeType(List<SavingProductFee> fees, string feeType, bool isMoralForm)
        {
            // Filter fees based on the fee type
            var filteredFees = fees.Where(fee => fee.Fee.FeeType.ToLower() == feeType.ToLower() && fee.Fee.IsMoralPerson == isMoralForm && fee.IsDeleted == false);

            // Return the first fee that matches the fee type
            return filteredFees.FirstOrDefault();
        }
        // Function to return the appropriate FeePolicyDto based on the amount
        public static FeePolicy GetFeePolicyRange(List<FeePolicy> feePolicies, decimal amount)
        {
            // Find the first FeePolicyDto where the amount falls within the range defined by AmountFrom and AmountTo
            var result = feePolicies.FirstOrDefault(policy => amount >= policy.AmountFrom && amount <= policy.AmountTo);

            if (result == null)
            {
                if (!feePolicies.Any())
                {
                    var errorMessage = $"Fee policy is not configured in the harmonize charge policy. Contact your system administrator to properly configure the ranges in the charge policy.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.

                }
                else
                {
                    decimal min = feePolicies.Min(policy => policy.AmountFrom);
                    decimal max = feePolicies.Max(policy => policy.AmountTo);
                    var errorMessage = $"The amount {BaseUtilities.FormatCurrency(amount)} does not lie between the configuration range {BaseUtilities.FormatCurrency(min)} & {BaseUtilities.FormatCurrency(max)} in the harmonize charge policy. Contact your system administrator to properly configure the ranges in the charge policy.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.

                }
            }

            return result;
        }
        // Function to select all holidays if true
        static List<string> GetHolidays(Config config)
        {
            var holidays = new List<string>();

            // Check each day property and add to holidays list if true
            if (config.MondayIsHoliday) holidays.Add("Monday");
            if (config.TuesDayIsHoliday) holidays.Add("Tuesday");
            if (config.MWednessdayIsHoliday) holidays.Add("Wednesday");
            if (config.ThursdayIsHoliday) holidays.Add("Thursday");
            if (config.FridayIsHoliday) holidays.Add("Friday");
            if (config.SaturdayIsHoliday) holidays.Add("Saturday");
            if (config.SundayIsHoliday) holidays.Add("Sunday");

            return holidays;
        }
        public static string DetermineOperationType(bool isInterBranchOperation, string operationType)
        {


            //WithdrawalSWS
            if (operationType.ToLower() == "cashin" || operationType.ToLower() == "remittancein"|| operationType.ToLower() == "remittanceout" || operationType.ToLower() == "deposit" || operationType.ToLower() == "deposit" || operationType.ToLower() == "withdrawal" || operationType.ToLower() == "loanrepayment" || operationType.ToLower() == "cashinmomocashcollection" || operationType.ToLower() == "loanrepaymentmomocashcollection")
            {
                operationType = BulkOperationType.CASH.ToString();
            }
            else if (operationType.ToLower() == "withdrawal_check" || operationType.ToLower() == "withdrawalsws")
            {
                operationType = BulkOperationType.CHECK.ToString();
            }
            BulkOperationType type;
            if (Enum.TryParse(operationType, out type))
            {
                switch (type)
                {
                    case BulkOperationType.CASH:
                        return isInterBranchOperation ? InterOperationType.Inter_Branch_Cash.ToString() : InterOperationType.Local_Cash.ToString();
                    case BulkOperationType.CHECK:
                        return isInterBranchOperation ? InterOperationType.Inter_Branch_Check.ToString() : InterOperationType.Local_Check.ToString();
                }
            }

            return "N/A";
        }
        public static TransactionDto MapTransactionToDto(Transaction transaction, List<CurrencyNotesDto> currencyNotes, UserInfoToken _userInfoToken)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                OriginalDepositAmount = transaction.OriginalDepositAmount,
                AccountId = transaction.AccountId,
                AccountNumber = transaction.AccountNumber,
                TransactionType = transaction.TransactionType,
                Operation = transaction.Operation,
                Status = transaction.Status,
                TransactionReference = transaction.TransactionReference,
                Tax = transaction.Tax,
                AccountType = transaction.Account?.AccountType,
                CreatedDate = transaction.CreatedDate, // Assuming it's correctly mapped
                PreviousBalance = transaction.PreviousBalance,
                Note = transaction.Note,
                SenderAccountId = transaction.SenderAccountId,
                ReceiverAccountId = transaction.ReceiverAccountId,
                DepositorIDNumber = transaction.DepositorIDNumber,
                DepositorIDNumberPlaceOfIssue = transaction.DepositorIDNumberPlaceOfIssue,
                DepositerNote = transaction.DepositerNote,
                DepositorName = transaction.DepositorName,
                DepositorIDIssueDate = transaction.DepositorIDIssueDate,
                IsDepositDoneByAccountOwner = transaction.IsDepositDoneByAccountOwner,
                DepositerTelephone = transaction.DepositerTelephone,
                DepositorIDExpiryDate = transaction.DepositorIDExpiryDate,
                Balance = transaction.Balance,
                ProductId = transaction.ProductId,
                Fee = transaction.Fee,
                FeeType = transaction.FeeType,
                BankId = transaction.BankId,
                Credit = transaction.Credit,
                AmountInWord = $"{BaseUtilities.ConvertToWords(transaction.OriginalDepositAmount + transaction.Fee)}",
                BranchId = transaction.BranchId,
                TellerId = transaction.TellerId,
                DestinationBrachId = transaction.DestinationBrachId,
                DestinationBranchCommission = transaction.DestinationBranchCommission,
                Account = transaction.Account,
                IsInterBrachOperation = transaction.IsInterBrachOperation,
                OperationType = transaction.OperationType,
                Debit = transaction.Debit,
                SourceBrachId = transaction.SourceBrachId,
                SourceBranchCommission = transaction.SourceBranchCommission,
                Teller = transaction.Teller,
                CloseOfAccountCharge = transaction.CloseOfAccountCharge,
                OperationCharge = transaction.OperationCharge,
                WithdrawalChargeWithoutNotification = transaction.WithdrawalChargeWithoutNotification,
                WithrawalFormCharge = transaction.WithrawalFormCharge,
                TellerOperations = transaction.TellerOperations,
                Currency = transaction.Currency,
                CustomerId = transaction.CustomerId,
                ExternalApplicationName = transaction.ExternalApplicationName,
                ExternalReference = transaction.ExternalReference,
                IsExternalOperation = transaction.IsExternalOperation,
                SourceType = transaction.SourceType,
                CreatedBy = _userInfoToken.Id,
                CurrencyNotes = currencyNotes,
                ReceiptTitle = transaction.ReceiptTitle,
                HeadOfficeCommission=transaction.HeadOfficeCommission,
                AccountingDate=transaction.AccountingDate,
                CamCCULCommission=transaction.CamCCULCommission,
                FluxAndPTMCommission=transaction.FluxAndPTMCommission,
            };
        }

        public static OtherTransactionDto MapOtherTransaction(OtherTransaction otherTransaction)
        {
            return new OtherTransactionDto
            {
                Id = otherTransaction.Id,
                TransactionReference = otherTransaction.TransactionReference,
                EnventName = otherTransaction.EnventName,
                Description = otherTransaction.Description,
                TellerId = otherTransaction.TellerId,
                Amount = Math.Abs(otherTransaction.Amount),
                Debit = otherTransaction.Debit,
                AccountNumber = otherTransaction.AccountNumber,
                EventCode = otherTransaction.EventCode,
                MemberName = otherTransaction.MemberName, // Assuming MemberName maps to Name
                Credit = otherTransaction.Credit,
                Direction = otherTransaction.Direction,
                TransactionType = otherTransaction.TransactionType,
                SourceType = otherTransaction.SourceType,
                Narration = otherTransaction.Narration,
                CustomerId = otherTransaction.CustomerId,
                BranchId = otherTransaction.BranchId,
                BankId = otherTransaction.BankId,
                Teller = otherTransaction.Teller,
                AmountInWord = otherTransaction.AmountInWord,
                ReceiptTitle = otherTransaction.ReceiptTitle,
                DateOfOperation = otherTransaction.DateOfOperation,
                TelephoneNumber = otherTransaction.TelephoneNumber,
                CNI = otherTransaction.CNI,
            };
        }
        public static Transaction MapTransaction(OtherTransaction transaction, UserInfoToken _userInfoToken, Account account)
        {
            string N_A = "N/A";
            return new Transaction
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                OriginalDepositAmount = transaction.Amount,
                AccountId = account.Id != null ? account.Id : N_A,
                AccountNumber = transaction.AccountNumber,
                TransactionType = transaction.TransactionType,
                Operation = "Other_Cash_IN",
                Status = "Approved",
                TransactionReference = transaction.TransactionReference,
                Tax = 0,
                CreatedDate = transaction.CreatedDate, // Assuming it's correctly mapped
                PreviousBalance = account.PreviousBalance,
                Note = transaction.Narration,
                SenderAccountId = N_A,
                ReceiverAccountId = N_A,
                DepositorIDNumber = N_A,
                DepositorIDNumberPlaceOfIssue = N_A,
                DepositerNote = N_A,
                DepositorName = N_A,
                DepositorIDIssueDate = N_A,
                IsDepositDoneByAccountOwner = true,
                DepositerTelephone = N_A,
                DepositorIDExpiryDate = N_A,
                Balance = account.Balance,
                ProductId = account.ProductId != null ? account.ProductId : N_A,
                Fee = 0,
                FeeType = N_A,
                BankId = transaction.BankId,
                Credit = transaction.Credit,
                BranchId = transaction.BranchId,
                TellerId = transaction.TellerId,
                DestinationBrachId = transaction.BranchId,
                DestinationBranchCommission = 0,
                Account = account,
                IsInterBrachOperation = false,
                OperationType = transaction.TransactionType,
                Debit = transaction.Debit,
                SourceBrachId = transaction.BranchId,
                SourceBranchCommission = 0,
                Teller = transaction.Teller,
                CloseOfAccountCharge = 0,
                OperationCharge = 0,
                WithdrawalChargeWithoutNotification = 0,
                WithrawalFormCharge = 0,
                TellerOperations = null,
                Currency = Currency.XAF.ToString(),
                CustomerId = transaction.CustomerId,
                ExternalApplicationName = N_A,
                ExternalReference = N_A,
                IsExternalOperation = false,
                SourceType = transaction.SourceType,
                CreatedBy = _userInfoToken.Id,
                ReceiptTitle = $"Cash-Receipt {transaction.EnventName}: Reference: " + transaction.TransactionReference,
            };
        }
        public static Transaction MapTransactionMobileMoney(OtherTransaction transaction, UserInfoToken _userInfoToken, Account account, OtherTransactionDto otherTransactionDto)
        {
            string N_A = "N/A";
            return new Transaction
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                OriginalDepositAmount = transaction.Amount,
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                TransactionType = transaction.TransactionType,
                Operation = "Other_Cash_IN",
                Status = "Approved",
                IsSWS = false,
                CheckName = N_A,
                CheckNumber = N_A,
                AccountingDate = transaction.DateOfOperation,
                TransactionReference = transaction.TransactionReference,
                Tax = 0,
                CreatedDate = transaction.CreatedDate, // Assuming it's correctly mapped
                PreviousBalance = account.PreviousBalance,
                Note = $"Prepresentative: {transaction.MemberName}, Tel: {transaction.TelephoneNumber}, CNI: {transaction.CNI}: [Operation: {transaction.Narration}], Reference: {transaction.TransactionReference}",
                SenderAccountId = N_A,
                ReceiverAccountId = N_A,
                DepositorIDNumber = N_A,
                DepositorIDNumberPlaceOfIssue = N_A,
                DepositerNote = N_A,
                DepositorName = transaction.MemberName,
                DepositorIDIssueDate = N_A,
                IsDepositDoneByAccountOwner = true,
                DepositerTelephone = transaction.TelephoneNumber,
                DepositorIDExpiryDate = N_A,
                Balance = account.Balance,
                ProductId = account.ProductId != null ? account.ProductId : N_A,
                Fee = 0,
                FeeType = N_A,
                BankId = transaction.BankId,
                Credit = transaction.Debit,
                BranchId = transaction.BranchId,
                TellerId = transaction.TellerId,
                DestinationBrachId = transaction.BranchId,
                DestinationBranchCommission = 0,
                Account = account,
                IsInterBrachOperation = false,
                OperationType = transaction.EnventName,
                Debit = transaction.Credit,
                SourceBrachId = transaction.BranchId,
                SourceBranchCommission = 0,
                Teller = transaction.Teller,
                CloseOfAccountCharge = 0,
                OperationCharge = 0,
                WithdrawalChargeWithoutNotification = 0,
                WithrawalFormCharge = 0,
                TellerOperations = null,
                Currency = Currency.XAF.ToString(),
                CustomerId = transaction.CustomerId,
                ExternalApplicationName = N_A,
                ExternalReference = N_A,
                IsExternalOperation = false,
                SourceType = transaction.SourceType,
                CreatedBy = _userInfoToken.Id,
                ReceiptTitle = otherTransactionDto.ReceiptTitle,
            };
        }

        public static Transaction MapTransactionMobileMoneyTopUp(MobileMoneyCashTopup transaction, UserInfoToken _userInfoToken, Account account, string TransactionType, DateTime accountingDate)
        {
            string N_A = "N/A";
            return new Transaction
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                OriginalDepositAmount = transaction.Amount,
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                TransactionType = "Credit",
                Operation = TransactionType,
                Status = "Approved",
                IsSWS = false,
                CheckName = N_A,
                CheckNumber = N_A,
                AccountingDate = accountingDate,
                TransactionReference = transaction.RequestReference,
                Tax = 0,
                CreatedDate = transaction.CreatedDate, // Assuming it's correctly mapped
                PreviousBalance = account.PreviousBalance,
                Note = $"TRANSFER: {TransactionType}, Tel: {transaction.PhoneNumber}, Amount: {BaseUtilities.FormatCurrency(transaction.Amount)}, Reference: {transaction.RequestReference}, MMC/OMC Transaction Code: {transaction.MobileMoneyTransactionId}, Member Reference: {transaction.RequestReference}",
                SenderAccountId = N_A,
                ReceiverAccountId = N_A,
                DepositorIDNumber = N_A,
                DepositorIDNumberPlaceOfIssue = N_A,
                DepositerNote = N_A,
                DepositorName = transaction.RequestApprovedBy,
                DepositorIDIssueDate = N_A,
                IsDepositDoneByAccountOwner = true,
                DepositerTelephone = N_A,
                DepositorIDExpiryDate = N_A,
                Balance = account.Balance,
                ProductId = account.ProductId,
                Fee = 0,
                FeeType = N_A,
                BankId = transaction.BankId,
                Credit = transaction.Amount,
                BranchId = transaction.BranchId,
                TellerId = transaction.TellerId,
                DestinationBrachId = transaction.BranchId,
                DestinationBranchCommission = 0,
                Account = account,
                IsInterBrachOperation = false,
                OperationType = "Account Top-Up",
                Debit = 0,
                SourceBrachId = transaction.BranchId,
                SourceBranchCommission = 0,
                CloseOfAccountCharge = 0,
                OperationCharge = 0,
                WithdrawalChargeWithoutNotification = 0,
                WithrawalFormCharge = 0,
                TellerOperations = null,
                Currency = Currency.XAF.ToString(),
                CustomerId = transaction.MobileMoneyMemberReference,
                ExternalApplicationName = N_A,
                ExternalReference = N_A,
                IsExternalOperation = false,
                SourceType = transaction.SourceType,
                CreatedBy = _userInfoToken.Id,
                ReceiptTitle = "Account Top-Up",
            };
        }

        public static TransactionThirdPartyDto MapToTransactionThirdPartyDto(TransactionDto transactionDto, string PhoneNumber, BranchDto branch, Teller teller)
        {
            return new TransactionThirdPartyDto
            {
                Id = transactionDto.Id,
                Amount = transactionDto.OriginalDepositAmount,
                AccountNumber = transactionDto.AccountNumber,
                Status = transactionDto.Status,
                TransactionReference = transactionDto.TransactionReference,
                TransactionType = transactionDto.TransactionType,
                Note = transactionDto.Note,
                TelephoneNumber = PhoneNumber,
                Fee = transactionDto.Fee,
                TotalCharge = transactionDto.Fee + transactionDto.Tax,
                TransactionDate = transactionDto.CreatedDate, // Placeholder value
                AmountInWord = transactionDto.AmountInWord, // Assuming AmountInWord is directly mapped
                BranchCode = branch.branchCode, // Assuming BranchCode maps to BranchId
                TellerCode = teller.Code, // Assuming TellerCode maps to TellerId
                ExternalReference = transactionDto.ExternalReference,
                ExternalApplicationName = transactionDto.ExternalApplicationName,
            };
        }
        public static BulkDeposit MapBulkOperationToBulkDeposit(BulkOperation bulkOperation, bool IsLoanRepayment, string accountNumber)
        {
            return new BulkDeposit
            {
                AccountNumber = IsLoanRepayment == false ? bulkOperation.AccountNumber : accountNumber,
                Fee = bulkOperation.Fee,
                CustomerId = bulkOperation.CustomerId,
                Amount = bulkOperation.Amount,
                Balance = bulkOperation.Balance,
                Penalty = bulkOperation.Penalty, Tax=bulkOperation.VAT,
                Interest = bulkOperation.Interest,
                Total = bulkOperation.Total,
                AccountType = bulkOperation.AccountType,
                LoanId = bulkOperation.LoanId,
                Note = bulkOperation.Note,
                OperationType = bulkOperation.OperationType,
                IsDepositDoneByAccountOwner = bulkOperation.isDepositDoneByAccountOwner,
                currencyNotesRequest = bulkOperation.currencyNotes,
                Depositer = bulkOperation.Depositer,
                Branch = new BranchDto(),
                currencyNotes = new List<CurrencyNotesDto>(),
                Currency = string.Empty,
                Customer = new CustomerDto(),
                CheckName = bulkOperation.CheckName,
                IsSWS = bulkOperation.IsSWS,
                CheckNumber = bulkOperation.CheckNumber,
                ExternalApplicationName = null,
                ExternalReference = null,
                IsExternalOperation = false,
                IsChargesInclussive = bulkOperation.IsChargesInclussive,
                SourceType = bulkOperation.SourceType,
                MembershipActivationAmount = bulkOperation.MembershipActivationAmount,
            };
        }
        public static List<BulkDeposit> MapBulkOperationListToBulkDepositList(List<BulkOperation> bulkOperationList, bool IsLoanRepayment = false, string accountNumber = null)
        {
            var bulkDepositList = new List<BulkDeposit>();

            foreach (var bulkOperation in bulkOperationList)
            {
                if (bulkOperation.AccountNumber != bulkOperation.CustomerId)
                {
                    var bulkDeposit = MapBulkOperationToBulkDeposit(bulkOperation, IsLoanRepayment, accountNumber);
                    bulkDepositList.Add(bulkDeposit);
                }
            }

            return bulkDepositList;
        }
        // Method to generate transaction reference
        public static string GenerateTransactionReference(UserInfoToken userInfoToken, string OperationName, bool IsinterBranch)
        {
            // Set the default prefix for the reference number, starting with "LB" (likely stands for Local Branch)
            string prefix = "LB";

            // Check the operation name to determine the specific prefix and operation type
            switch (OperationName.ToLower()) // Convert operation name to lowercase for case-insensitive comparison
            {
                case "withdrawal":
                    prefix += "W"; // Append "W" for withdrawals
                    break;
                case "deposit":
                    prefix += "D"; // Append "D" for deposits
                    break;
                case "transfer":
                    prefix += "T"; // Append "T" for transfers
                    break;
                case "ttp_transfer":
                    prefix += "TTP"; // Append "TTP" for Third-Party (TTP) transfers
                    break;
                case "lr_deposit":
                    prefix += "LOR"; // Append "LOR" for loan repayment deposits
                    break;
                case "cashin_loan_repayment":
                    prefix += "LR"; // Append "LOR" for cash-in loan repayments
                    break;
                case "loan_disburment":
                    prefix += "LDI"; // Append "LOD" for loan disbursements
                    break;
                case "loan_disbursement_refinance":
                    prefix += "RIF"; // Append "LOD" for loan disbursements
                    break;
                case "withdrawalrequest":
                    prefix += "SWR"; // Append "SWR" for withdrawal requests
                    break;
                case "othercashin_expense":
                    prefix += "OEX"; // Append "OEX" for other cash-in expenses
                    break;
                case "othercashin_income":
                    prefix += "OIN"; // Append "OIN" for other cash-in incomes
                    break;
                case "othercashin_income_withdrawal":
                    prefix += "OCW"; // Append "OCW" for other cash-in income withdrawals
                    break;
                case "ttp_withdrawal":
                    prefix += "WTTP"; // Append "WTTP" for Third-Party (TTP) withdrawals
                    break;
                case "loan_accrual_interest":
                    prefix += "LAI"; // Append "LOAI" for loan accrual interest
                    break;
                case "deposit_loan_repayment":
                    prefix += "LR"; // Append "LOR" for loan repayments via deposit
                    break;
                case "mmc_in":
                    prefix += "CIMM"; // Append "CIMMC" for Mobile Money cash-in
                    break;
                case "mmc_out":
                    prefix += "COMM"; // Append "COMMC" for Mobile Money cash-out
                    break;
                case "omc_in":
                    prefix += "CIOM"; // Append "CIOMC" for Orange Money cash-in
                    break;
                case "omc_out":
                    prefix += "COOM"; // Append "COOMC" for Orange Money cash-out
                    break;
                case "ttp_transfer_gav":
                    prefix += "TGAV"; // Append "TGAV" for Third-Party transfer to GAV
                    break;
                case "ttp_transfer_cmney":
                    prefix += "TCM"; // Append "TCMON" for Third-Party transfer to C-Money
                    break;
                case "momokcashcollection":
                    prefix += "MCC"; // Append "MCC" for MomoKash collections
                    break;
                case "momokcashcollection_loan_repayment":
                    prefix += "MLR"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "expense":
                    prefix += "OEX"; // Append "MCCLOR" for MomoKash loan repayments
                    break;

                case "income":
                    prefix += "OIN"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "cash_w_remittance":
                    prefix += "WRM"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "cash_in_remittance":
                    prefix += "CRM"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "vaultoperationchange":
                    prefix += "VTCC"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "cashchangeoperationsubtill":
                    prefix += "STCC"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "cashchangeoperationprimarytill":
                    prefix += "PTCC"; // Append "MCCLOR" for MomoKash loan repayments
                    break;
                case "salary":
                    prefix += "S"; // Append "S" for Salary
                    break;
                    
                default:
                    prefix += "O"; // Default case for other operations; append "O" (for Other)
                    break;
            }

            // Generate the unique reference number with prefix and branch code
            string reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"{prefix}-{userInfoToken.BranchCode}-");

            // If it's an inter-branch operation, adjust the prefix and regenerate the reference
            if (IsinterBranch)
            {
                prefix = "IB" + prefix.Substring(2); // Replace "LB" with "IB" for inter-branch operations
                reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"{prefix}-{userInfoToken.BranchCode}-"); // Regenerate the reference for inter-branch transactions
            }

            // Return the final reference number
            return reference;
        }
        public static string[] ExtractAccountNumberAndOriginalAmount(IEnumerable<TransactionDto> transactions)
        {
            // Extract AccountNumber and OriginalDepositAmount and format as "AccountNumber-OriginalAmount"
            var result = transactions.Select(t => $"{t.AccountNumber}-{t.OriginalDepositAmount}");
            return result.ToArray();
        }
        public static string ExtractAccountNumbersFromTransaction(IEnumerable<TransactionDto> transactions)
        {
            // Extract AccountNumber and OriginalDepositAmount and format as "AccountNumber-OriginalAmount"
            var resultArray = transactions.Select(t => $"{t.AccountNumber}");
            // Convert the array of strings to a single string separated by commas
            string resultString = string.Join(", ", resultArray);
            return resultString;
        }
        public static string ExtractAccountTypesAndAmountFromTransaction(IEnumerable<TransactionDto> transactions)
        {
            // Extract AccountNumber and OriginalDepositAmount and format as "AccountNumber-OriginalAmount"
            var resultArray = transactions.Select(t => $"{t.AccountType}:{t.Amount}");
            // Convert the array of strings to a single string separated by commas
            string resultString = string.Join(", ", resultArray);
            return resultString;
        }

        public static List<CurrencyNotes> ComputeCurrencyNote(CurrencyNotesRequest request, string ReferenceId)
        {
            List<CurrencyNotes> currencyNotesList = new List<CurrencyNotes>();

            // Define the values of each denomination using the enum
            Dictionary<XAFDenomination, int> denominationValues = new Dictionary<XAFDenomination, int>
        {
            { XAFDenomination.Note10000, 10000 },
            { XAFDenomination.Note5000, 5000 },
            { XAFDenomination.Note2000, 2000 },
            { XAFDenomination.Note1000, 1000 },
            { XAFDenomination.Note500, 500 },
            { XAFDenomination.Coin500, 500 },
            { XAFDenomination.Coin100, 100 },
            { XAFDenomination.Coin50, 50 },
            { XAFDenomination.Coin25, 25 },
            { XAFDenomination.Coin10, 10 },
            { XAFDenomination.Coin5, 5 },
            { XAFDenomination.Coin1, 1 }
        };

            // Calculate the total amount
            decimal grandTotal = 0;
            foreach (var kvp in denominationValues)
            {
                int quantity = (int)typeof(CurrencyNotesRequest).GetProperty(Enum.GetName(typeof(XAFDenomination), kvp.Key)).GetValue(request);
                int denominationValue = kvp.Value;
                decimal subtotal = quantity * denominationValue;
                if (quantity > 0)
                {
                    CurrencyNotes currencyNote = new CurrencyNotes
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique ID
                        DinominationType = Enum.GetName(typeof(XAFDenomination), kvp.Key).StartsWith("Note") ? "Note" : "Coin",
                        Denomination = denominationValue.ToString(),
                        Value = quantity,
                        SubTotal = subtotal,
                        Total = 0, // Set total to 0 initially
                        ReferenceId = ReferenceId
                    };
                    currencyNotesList.Add(currencyNote);
                    grandTotal += subtotal;
                }
            }

            // Update the total amount for each currency note
            foreach (var currencyNote in currencyNotesList)
            {
                currencyNote.Total = grandTotal;
            }

            return currencyNotesList;
        }

        public static CurrencyNotesRequest CalculateCurrencyNotes(decimal amount)
        {
            CurrencyNotesRequest result = new CurrencyNotesRequest();

            // Convert decimal amount to integer (cents) to avoid precision issues
            int remainingAmount = (int)(amount * 100);

            // Determine number of notes for each denomination
            result.Note10000 = remainingAmount / 1000000; // 10000 * 100 (to convert decimal to cents)
            remainingAmount %= 1000000;

            result.Note5000 = remainingAmount / 500000; // 5000 * 100 (to convert decimal to cents)
            remainingAmount %= 500000;

            result.Note2000 = remainingAmount / 200000; // 2000 * 100 (to convert decimal to cents)
            remainingAmount %= 200000;

            result.Note1000 = remainingAmount / 100000; // 1000 * 100 (to convert decimal to cents)
            remainingAmount %= 100000;

            result.Note500 = remainingAmount / 50000; // 500 * 100 (to convert decimal to cents)
            remainingAmount %= 50000;

            // Now handle coins
            result.Coin500 = remainingAmount / 50000; // 500 * 100 (to convert decimal to cents)
            remainingAmount %= 50000;

            result.Coin100 = remainingAmount / 10000; // 100 * 100 (to convert decimal to cents)
            remainingAmount %= 10000;

            result.Coin50 = remainingAmount / 5000; // 50 * 100 (to convert decimal to cents)
            remainingAmount %= 5000;

            result.Coin25 = remainingAmount / 2500; // 25 * 100 (to convert decimal to cents)
            remainingAmount %= 2500;

            result.Coin10 = remainingAmount / 1000; // 10 * 100 (to convert decimal to cents)
            remainingAmount %= 1000;

            result.Coin5 = remainingAmount / 500; // 5 * 100 (to convert decimal to cents)
            remainingAmount %= 500;

            result.Coin1 = remainingAmount / 100; // 1 * 100 (to convert decimal to cents)

            return result;
        }

    }

}

