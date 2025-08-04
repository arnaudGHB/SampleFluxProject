using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Enums
{
    public enum InterestType
    {
        Simple_Interest,
        //Compounded_Interest,
    }
    public enum InterestPeriod
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
    }
    //    Loan Target should be added when applying for a loan during loan Application
    //1. Employee
    //2. Government
    //3. Member
    //4. Company
    //5. Board of directors
    //6. Group
    //    Loan Category during loan Application
    //1. Main
    //2. Special saving facility
    public enum LoanCategories
    {
        Main_Loan,
        Special_Saving_Facility_Loan,
    }

    public enum LoanTargets
    {
        Elected_Staff,
        Individual,
        Moral,
        Employee,
        Private_Sectors,
        Public_Sectors,
        CamCCUL_Staff
    }
    public enum LoanTerms
    {
        Long_Term_Loan,
        Medium_Term_Loan,
        Short_Term_Loan,
    }
    public enum LoanType
    {
        Main_Loan,
        Exceptional_Loan,
        Staff_Or_Elected_Official_Loan,
        Special_Loans_Or_Advance_Salaries,
        Staff_Car_Loans,
        Project_Loans,
        Golden_Loans,
        Micro_Loans,
        Consortium_And_Partnership_Loans
    }
    


    public enum OperationEventRubbriqueName
    {

        Loan_Principal_Account,
        Loan_VAT_Account,
        Loan_Interest_Recieved_Account,
        Loan_Penalty_Account,
        Loan_WriteOff_Account,
        Loan_Provisioning_Account_MoreThanOneYear,
        Loan_Provisioning_Account_MoreThanTwoYear,
        Loan_Provisioning_Account_MoreThanThreeYear,
        Loan_Provisioning_Account_MoreThanFourYear,
        Loan_Transit_Account,
        Loan_Product,
    }
    public enum LoanOperationType
    {

        Disbursment,
        Approval,
        Cancellation,
    }
    public enum LoanDurationPeriod
    {
        Days,
        Weeks,
        Months,
        Years,

    }
    public enum LoanDisburstmentType
    {
        Cash,
        Transfer,
        Check
    }
    //
    public enum RefundOrder
    {
        Interest = 0,
        Principal = 1,
        Charges = 2
    }
    public enum YesOrNo
    {
        Yes,
        No,
    }
    public enum PenaltyTypes
    {
        Late_Repayment_Penalty,
        Penalty_After_Maturity_Date,
    }
    public enum CalculateInterestOn
    {
        Overdue_Principal_Amount,
        Overdue_Interest_Amount,
        Overdue_Principal_Plus_Interest_Plus_Amount,
        Overdue_Principal_Plus_Interest_Plus_Fees_Amount,
        Overdue_Principal_Plus_Interest_Plus_Penalty_Amount,
        Overdue_Principal_Plus_Interest_Plus_Fees_Plus_Penalty_Amount,
        Overdue_Interest_Fees_Amount,
        Total_Principal_Amount_Released
    }
    public enum RepaymentCycle
    {
        Daily,
        Weekly,
        Biweekly,
        Monthly,// Done.
        Bimonthly,
        Quarterly,
        Every4Months,
        SemiAnnual,
        Every9Months,
        Yearly,
        LumpSum
    }

    
    
    public enum LoanApplicationStatus
    {
        Rejected,
        Approved,
        Pending,
    }
    public enum LoanApplicationStatusX
    {
        Awaits_Loan_Commitee_Validation,
        Await_Initialization_Fee_Payment,
        Rejected,
        Under_review_By_Loan_Comminitee,
        Approved,
        Validated,
    }
    public enum LoanCommiteeValidationStatuses
    {
        Rejected,
        Understudies,
        Approved,
    }
    public enum AmortizationType
    {
        Constant_Amortization,
        Constant_Annuity,

    }
    public enum OTPStatuses
    {
        Approved, Expired,
    }
    public enum DisbursmentStatus
    {
        Disbursed,
        Cancelled,
        Pending,
    }
    //Normal,Bad loan, Due loan, Over due loan, unracoverable loan, Write off loan
    public enum LoanStatus
    {
        Open,
        Closed,
        Disbursed,
        Restructured,
        NormalLoans,
        DeliquentLoans,
        BadLoans,
        DueLoans,
        OverDueLoans,
        WriteOffLoans,
        DefaultedLoans, Refinancing,
        Rescheduled,
    }
    public enum LoanDeliquentStatus
    {
        NormalLoans, 
        Current, 
        Delinquent,
        DueLoansEarlyStageDelinquency,
        MinorDelinquency,
        ModerateDelinquency,
        SevereDelinquency,
        CriticalDelinquency,
        SeriousDelinquency,
        BadLoans,
        WriteOffLoans,
        Defaulted
    }
    public enum PaymentModes
    {
        Cash,
        Transfer,
        Check,
        Online_Payment,
        Orange_Money,
        MTN_Money,
        GAV
    }



    public enum LoanApplicationTypes
    {
        Normal,
        Refinancing,
        Reschedule,
        Restructure
    }
    public enum GurantorsTypes
    {
        Co_Obligor,
        Inter_Cooperation,
        Moral_Guarantor,
        Shotee
    }

}

