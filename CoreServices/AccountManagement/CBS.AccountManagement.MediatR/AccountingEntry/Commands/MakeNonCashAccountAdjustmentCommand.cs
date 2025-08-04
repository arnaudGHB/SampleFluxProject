using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    

    public class MakeNonCashAccountAdjustmentCommand : IRequest<ServiceResponse<bool>>
    {
        public string ChartOfAccountId { get; set; }
        public string ProductId { get; set; }
        public string Source { get; set; } //ProductId or ChartOfAccountId
        public string BookingDirection { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public string TransactionReference { get; set; }
        public string MemberReference { get; set; }
        public DateTime TransactionDate { get;  set; }

        public string? GetOperationEventCode(string productId)
        {
            return $"{productId}@Principal_Saving_Account";
        }
    }
}
