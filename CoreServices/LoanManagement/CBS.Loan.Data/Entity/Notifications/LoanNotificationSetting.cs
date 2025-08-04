using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.Notifications
{
    public class LoanNotificationSetting:BaseEntity
    {
        public int Id { get; set; }
        public bool SendNotificationToMemberAfterLoanApproval { get; set; }
        public bool SendNotificationOTPToMemberForLoanApproval { get; set; }
        public bool SendNotificationOTPToBranchMsisdnForLoanApproval { get; set; }
        public bool SendNotificationToMemberAfterLoanApplication { get; set; }
        public string BranchTelephoneForOTP{ get; set; }
        public string BranchEmailForOTP { get; set; }
        public string BranchId { get; set; }
        public bool SendNotificationToMemberAfterLoanRejection { get; set; }
        public bool SendNotificationBySMS { get; set; }
        public bool SendNotificationByEmail { get; set; }
        public bool SendNotificationNoBothSMSAndEmail { get; set; }
        public int ExprireTimeOfOTPInMinutes { get; set; }
    }
}
