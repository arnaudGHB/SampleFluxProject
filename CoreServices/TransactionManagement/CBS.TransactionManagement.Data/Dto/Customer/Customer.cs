using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.Dto
{
    public class CustomerDto
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? SignatureUrl { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string Matricule { get; set; }
        public string? IDNumber { get; set; }
        public string? IDNumberIssueDate { get; set; }
        public string? IDNumberIssueAt { get; set; }
        public string Language { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MembershipApprovalStatus { get; set; }
        public bool Active { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string LegalForm { get; set; }
        public string BranchCode { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
        public string? AgeCategoryStatus { get; set; }

    }
    public class CustomerKYCDto
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
    public class MemberActivationPolicyDto
    {
        public string EventCodeEntranceFee { get; set; }
        public string EventCodeByeLawsFee { get; set; }
        public string EventCodeLoanPolicyFee { get; set; }
        public string EventCodeBuildingContributionFee { get; set; }
        public decimal EntranceFee { get; set; }
        public decimal ByeLawsFee { get; set; }
        public decimal LoanPolicyFee { get; set; }
        public decimal BuildingContribution { get; set; }
        public decimal Total { get; set; }
        public string PolicyID { get; set; }

    }
    public class MemberFeePolicyDto
    {
       
        public decimal Amount { get; set; }
        public List<FeePolicy> Policies { get; set; }
        public List<MemberAccountActivation> MemberAccountActivations { get; set; }
        public MemberFeePolicyDto()
        {
            Policies = new List<FeePolicy>();
            MemberAccountActivations = new List<MemberAccountActivation>();
        }
    }
    public class CustomerPinValidationDto
    {
        public string Telephone { get; set; }
        public int NumberOfFailedTries { get; set; }
        public bool ValidationStatus { get; set; }
        public string LoginStatus { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public CustomerDto Customer { get; set; }
    }
}