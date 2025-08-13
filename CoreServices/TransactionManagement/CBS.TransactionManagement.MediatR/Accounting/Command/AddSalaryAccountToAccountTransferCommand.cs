using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Accounting.Command
{
    public class AddSalaryAccountToAccountTransferCommand : IRequest<ServiceResponse<bool>>
    {
        public string ReferenceCode { get; set; }
        public DateTime AccountingDate { get; set; }
        public UserInfoToken UserInfoToken { get; set; }
        public AddSalaryAccountToAccountTransferCommand()
        {
            UserInfoToken=new UserInfoToken();
        }
    }

}




