using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.LoanRepayment.Command
{
    public class AddLoanSSOBulkRepaymentCommand : IRequest<ServiceResponse<bool>>
    {
        public string SalaryCode { get; set; }
        public UserInfoToken UserInfoToken { get; set; }
        
    }

}




