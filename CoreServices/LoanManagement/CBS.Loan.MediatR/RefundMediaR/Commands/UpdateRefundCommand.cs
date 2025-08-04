using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RefundMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateRefundCommand : IRequest<ServiceResponse<RefundDto>>
    {
        public string Id { get; set; }
        public string InstallmentId { get; set; }
        public string Comment { get; set; }
        public string PaymentType { get; set; }
    }

}
