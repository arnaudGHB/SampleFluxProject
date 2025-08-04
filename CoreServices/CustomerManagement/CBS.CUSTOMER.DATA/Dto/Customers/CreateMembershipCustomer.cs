
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateMembershipCustomer 
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string? IDNumber { get; set; }
        public string? IDNumberIssueDate { get; set; }
        public string? IDNumberIssueAt { get; set; }
        public string? MembershipApplicantDate { get; set; }
        public string? MembershipApplicantProposedByReferral1 { get; set; }
        public string? MembershipApplicantProposedByReferral2 { get; set; }
        public string? MembershipApprovalBy { get; set; }
        public string? MembershipApprovalStatus { get; set; }
        public string? MembershipApprovedDate { get; set; }
        public string? MembershipApprovedSignatureUrl { get; set; }
        public string? MembershipAllocatedNumber { get; set; }
        public string? LegalForm { get; set; }

    }
}
