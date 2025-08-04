using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanDeliquencyConfiguration : BaseEntity
    {
        public string Id { get; set; }
        public int DaysFrom { get; set; }
        public int DaysTo { get; set; }
        public string Status { get; set; }//Normal,Bad loan, Due loan, Over due loan, unracoverable loan, Write off loan
        public string Name { get; set; }// Par 0-30, Par 31-60, Par 61-90, Par 91-120
        public bool SendSMStoClient { get; set; }
        public bool SendMail { get; set; }
        public bool SendSMS { get; set; }
        public bool ApplyFine { get; set; }
        public bool AffectScoring { get; set; }
        public bool ReportToCreditOffice { get; set; }

    }
}
