using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.Notifications
{
    public class OTPNotificationDto
    {
        public string Id { get; set; }
        public string OPTCode { get; set; }
        public DateTime InitializedDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string Status { get; set; }//OK Or Expired
        public string LoanApplicationId { get; set; }
        public string InitiatedBy { get; set; }
        public string CustomerId { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }
    public class TempOTPDto
    {
        public string Otp { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Url { get; set; }

        public string Id { get; set; }
        public bool IsVerify { get; set; } = true;
    }
    public class TempVerificationOTPDto
    {
        public string Id { get; set; }
        public bool IsVerify { get; set; } = true;
    }
}
