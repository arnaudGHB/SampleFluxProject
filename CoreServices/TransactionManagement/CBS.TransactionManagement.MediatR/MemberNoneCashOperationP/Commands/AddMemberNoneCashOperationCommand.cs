using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>
    public class AddMemberNoneCashOperationCommand : IRequest<ServiceResponse<MemberNoneCashOperationDto>>
    {
        public string MemberReference { get; set; }
        public string AccountNUmber { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public string ChartOfAccountId { get; set; }
        public string BookingDirection { get; set; }
        public string MemberName { get; set; }
        public string ChartOfAccountName { get; set; }
    }
    
}
