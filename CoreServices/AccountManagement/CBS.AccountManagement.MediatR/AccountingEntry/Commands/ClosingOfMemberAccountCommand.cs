using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class ClosingOfMemberAccountCommand : IRequest<ServiceResponse<bool>>
    {
        //DailyCollector
        public string TransactionReference { get; set; }
        public string? MemberReference { get; set; }
        public string EventCode { get; set; }
        public List<MemberAccountDetail> MemberAccountDetails { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class MemberAccountDetail
    {
    
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string EventAttributeName { get; set; }
        public decimal Amount { get; set; }

        public string GetOperationEventCode(string productName)
        {

            return productName + "@" + this.EventAttributeName;
        }
    }
}
