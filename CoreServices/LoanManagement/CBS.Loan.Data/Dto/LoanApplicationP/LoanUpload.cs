namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class LoanUpload
    {
        public string Folder { get; set; }
        public string MemberNumber { get; set; }
        public string MemberName { get; set; }
        public DateTime SubmitDate { get; set; }
        public DateTime ValidationDate { get; set; }
        public string LoanProduct { get; set; }
        public string Label { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanBalance { get; set; }
        public decimal DelayInterest { get; set; }
        public string Reason { get; set; }
        public string LoanAccount { get; set; }
        public string Periodic { get; set; }
        public int NBRECHE { get; set; }
        public string CalculationType { get; set; }
        public decimal IntRate { get; set; }
        public string PremierEcheance { get; set; }
        public string BranchCode { get; set; }
        public DateTime LastRepayment { get; set; }
        public string EndDate { get; set; }
        public string ANNEE { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public decimal Interest { get; set; }
        public string Description { get; set; }
        public int DeliquentDays { get; set; }
        public string DeliquentStatus { get; set; }
        public decimal DeliquentAmount { get; set; }
        public int AdvanceDays { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string LoanType { get; set; }
    }

}