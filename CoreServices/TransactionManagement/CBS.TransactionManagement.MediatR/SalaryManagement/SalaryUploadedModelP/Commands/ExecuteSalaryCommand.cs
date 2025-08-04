using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands
{
    public class ExecuteSalaryCommand : IRequest<ServiceResponse<SalaryProcessingDto>>
    {
        public string FileUploadId { get; set; }//Normal, LoanFeePayment, Disbursment, LoanRepayment
        public string MemberReferenceNumber { get; set; }
        public UserInfoToken? UserInfoToken { get; set; }
        public ExecuteSalaryCommand()
        {
            UserInfoToken=new UserInfoToken();
        }
    }
}
