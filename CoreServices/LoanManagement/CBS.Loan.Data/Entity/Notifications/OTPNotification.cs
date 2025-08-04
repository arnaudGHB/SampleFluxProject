using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.Notifications
{
    public class OTPNotification:BaseEntity
    {
        public string Id { get; set; }
        public string OPTCode { get; set; }
        public DateTime InitializedDate { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public string Status { get; set; }//Used Or Expired
        public string LoanApplicationId { get; set; }
        public string CustomerId { get; set; }
        public string InitiatedBy { get; set; }
        
        public virtual LoanApplication LoanApplication { get; set; }
    }
}
