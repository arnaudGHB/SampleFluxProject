using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Commands
{
    public class AddLoanApprovalCommand : IRequest<ServiceResponse<LoanApplicationDto>>
    {
        public string Id { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
        public string OTPCode { get; set; }
    }
}
