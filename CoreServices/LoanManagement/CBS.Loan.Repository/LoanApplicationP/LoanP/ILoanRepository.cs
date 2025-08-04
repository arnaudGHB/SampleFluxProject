using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Data.Dto.Resources;
using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanP
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<LoanList> GetLoansAsync(LoanResource loanResource);
        //Task<FileDownloadInfoDto> ExportLoansAsync();
        Task<FileDownloadInfoDto> ExportLoansAsync(string branchid, DateTime startDate, DateTime endDate, string branchName, string branchCode,string queryParameter,bool IsUnpaidOnly);
        Task<FileDownloadInfoDto> ExportLoansAsync(DateTime startDate, DateTime endDate, string branchName, string branchCode,string queryParameter, bool IsUnpaidOnly);
    }
}
