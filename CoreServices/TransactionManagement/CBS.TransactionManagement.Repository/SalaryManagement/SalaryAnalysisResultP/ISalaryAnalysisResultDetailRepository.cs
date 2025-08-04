using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP
{
    public interface ISalaryAnalysisResultDetailRepository : IGenericRepository<SalaryAnalysisResultDetail>
    {
        Task<SalaryAnalysisResultDto> AnalyzeSalaryFileAsync(string fileUploadId, LoanRepaymentOrderDto loanRepaymentOrder, BranchDto branch, List<SalaryUploadModel> salaryUploadModels, List<LightLoanDto> loans, List<CustomerDto> members);

    }
}
