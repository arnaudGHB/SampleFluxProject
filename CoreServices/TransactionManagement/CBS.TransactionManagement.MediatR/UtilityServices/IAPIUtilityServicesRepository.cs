using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.MediatR.UtilityServices
{
    public interface IAPIUtilityServicesRepository
    {
        Task<BranchDto> GetBranch(string branchid);
        Task<CustomerDto> GetCustomer(string customerId);
        Task<List<CustomerDto>> GetMultipleMembers(List<string> Matricules, string OperationType, bool isMatricule);
        Task<List<Loan>> GetMembersLoans(List<string> CustomerIds, string OperationType, string loanStatus);
        Task<List<BranchDto>> GetBranches();
        Task CreatAccountingRLog(string CommandJsonObject, CommandDataType commandDataType, string TransactionReferenceId, DateTime accountingDate, string destinationUrl);
        Task<List<CustomerDto>> GetMembersWithMatriculeByBranchQuery(string branchid);
        Task PushNotification(string MemberReference, PushNotificationTitle pushNotificationTitle, string NotificationBody);
        Task<LoanRepaymentOrderDto> GetSalaryLoanRepaymentOrder();
        void SalaryStandingOrderLoanRepayment(string salaryCode, UserInfoToken userInfoToken);
        void SalaryAccountToAccountTransfer(string transactionReference, DateTime accountingDate, UserInfoToken userInfoToken);
        Task<bool> CreateLoanAccount(string memberId, UserInfoToken userInfoToken);
        Task<List<LightLoanDto>> GetOpenLoansByBranchQuery(string branchid);
        Task CreatAccountingRLogBG(string CommandJsonObject, CommandDataType commandDataType, string TransactionReferenceId, DateTime accountingDate, string destinationUrl, UserInfoToken userInfoToken);
        Task<List<CustomerDto>> GetAllMembers(string branchid = "All", string queryParamter = "bymatricule");
    }
}