using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.CMoneyP
{
    public class CMoneySubscriptionDetailsDto
    {
        public string Id { get; set; }
        public string CMoneyMembersActivationAccountId { get; set; }
        public DateTime DateTime { get; set; }
        public string ActionType { get; set; }//New Subcription, Renewal,
        public decimal Amount { get; set; }
        public string BranchId { get; set; }
        public string MemberId { get; set; }
        public CMoneyMembersActivationAccount CMoneyMembersActivationAccount { get; set; }
    }
}
