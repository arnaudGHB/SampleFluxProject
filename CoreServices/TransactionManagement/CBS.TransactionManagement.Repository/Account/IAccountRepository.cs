using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.TransactionManagement.Repository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> RetrieveTellerAccount(Teller teller);
        //Task<FileDownloadInfoDto> ExportMemberAccountSummaryAsync(BranchDto branch);
        Task<Account> GetAccount(string accountNumber, string operationType);
        Task<bool> CheckBalanceForWithdrawalWithOutCharges(decimal Amount, Account account, string memberType, InterOperationType interOperationType);
        bool CheckBalanceForWithdrawalWithCharges(decimal Amount, Account account, WithdrawalParameter withdrawalParameter, decimal TotalAmountToWithdrawWithCharges, string memberType);
        void UpdateAccountNumber(Account account);
        void UpdateAccountNumber(List<Account> accounts);
        void DeleteAccount(Account account);
        bool CheckBalanceForTransferCharges(
            decimal amount,
            Account account,
            TransferParameter transferParameter,
            decimal totalAmountWithCharges,
            string memberType);
        Task<Account> GetRemittanceAccount(string accountNumber, string accountType, string operationType);
        Task<RemittanceChargeDto> CalculateRemittanceTransferCharges(decimal amount, string productid, RemittanceTypes transferType, string senderBranchid,string transfterType,bool isMinorAccount=false);
        Task<Account> RetrieveMobileMoneyTellerAccountByTellerCodeAndBranchId(string branchid, string tellerCode);
        Task<Account> GetAccountByAccountNumber(string accountNumber, string accountType);
        Task<Account> GetMemberAccount(string customerReference, string accountType);
        void ResetAccountBalance(Account account);
        Task<FileDownloadInfoDto> ExportCustomerBalancesAsync();
        Task<bool> CheckIfMemberHaveLoanAccount(string customerId);
        Task<FileDownloadInfoDto> ExportCustomerBalancesByBranchIdAsync(BranchDto branch);
        bool VerifyBalanceIntegrity(Account account);
        Task<MemberAccountSituationListing> GetMembersAccountSummaries(AccountResource accountResource);
        Task<Account> CheckTellerBalance(Teller teller, decimal Amount);
        Task<Account> GetGavAccount(string accountNumber);
        void DebitAccount(Account account, decimal amount,string lastOperationType= "Debit Operation");
        Task<Account> Get3PPDepositAccount(string customerId);
        Task<Account> RetrieveMobileMoneyTellerAccount(string branchid, string tellerType);
        void CreditAccount(Account account, decimal amount, string lastOperationType = "Credit Operation");
        Task<Account> RetrieveNoneMemberMobileMoneyAccountByMemberReference(string MemberReference);
        Task<FileDownloadInfoDto> ExportMemberAccountSummaryAsync(BranchDto branch, bool isBranch);
        Task ClosseDay(Account account);
        void ResetAccountBalance(Account account, decimal amount);
        Task<Account> GetAccountByAccountNumber(string accountNumber);
        Task OpenDay(string Reference, Account account, DateTime accountingDay);
        Task<Account> GetMemberLoanAccount(string customerId);
        Task<Account> RetrieveMobileMoneyTellerAccountAndCheckBalance(string branchid, string tellerType, decimal amount);
        DetermineTransferTypeDto DetermineTransferType(Account sourceAccount, Account destinationAccount);
        void CreditAndDebitAccount(Account account, decimal creditAmount, decimal debitAmount);
            /// <summary>
            /// Validates whether a specified account meets the necessary constraints for a given operation type on either 
            /// a Mobile or Third-Party app, and for a specified service type.
            /// </summary>
            /// <param name="account">The account object containing details and permissions of the product.</param>
            /// <param name="operationType">The type of transaction operation being performed (Transfer, CashIn, CashOut).</param>
            /// <param name="appType">Specifies whether the operation is on the Mobile App or a Third-Party App.</param>
            /// <param name="serviceType">The service for which the operation is being performed (GAV, CMONEY).</param>
            /// <exception cref="InvalidOperationException">Thrown if the account is not activated for the app or if 
            /// the operation type is not permitted for the account.</exception>
        void CheckAppConstraints(Account account, TransactionType operationType, AppType appType, ServiceType serviceType);
        /// <summary>
        /// Calculates the transfer charges for a given amount based on the transfer parameters and fee configuration.
        /// </summary>
        /// <param name="amount">The amount for which the transfer charges need to be calculated.</param>
        /// <param name="transferParameter">The transfer parameters containing the product and fee information.</param>
        /// <param name="transferType">The type of transfer (e.g., local transfer or inter-brnach transfer).</param>
        /// <returns>A TransferCharges object containing the calculated service charge and total charges.</returns>
        Task<TransferCharges> CalculateTransferCharges(decimal amount, string productid, FeeOperationType transferType, string senderBranchid, string transferType_local_Inter_branch,bool isSavingAccount=false, bool IsWithdrawalNotified = false, decimal formFee=0, bool isMinorAccount=false);
        Account CreditAccountBalanceReturned(Account account, decimal amount, string lastOperationType);
        Account DebitAccountBalanceReturned(Account account, decimal amount, string lastOperationType);
    }


}