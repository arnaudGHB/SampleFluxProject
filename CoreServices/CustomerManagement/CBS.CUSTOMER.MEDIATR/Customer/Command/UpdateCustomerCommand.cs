using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
        public class UpdateCustomerCommand : IRequest<ServiceResponse<UpdateCustomer>>
        {


        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsMemberOfACompany { get; set; }
        public bool IsMemberOfAGroup { get; set; }
        public string? Matricule { get; set; }
        public string? AccountConfirmationNumber { get; set; }
        public string? LegalForm { get; set; }
        public string? VillageOfOrigin { get; set; }
        public string? FormalOrInformalSector { get; set; }
        public string? BankingRelationship { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string? IdNumber { get; set; }
        public string? IdNumberIssueDate { get; set; }
        public string? IdNumberIssueAt { get; set; }
        public string? MembershipApplicantDate { get; set; }
        public string? MembershipApplicantProposedByReferral1 { get; set; }
        public string? MembershipApplicantProposedByReferral2 { get; set; }
        public string? MembershipApprovalBy { get; set; }
        public string? MembershipApprovalStatus { get; set; }
        public string? MembershipApprovedDate { get; set; }
        public string? MembershipApprovedSignatureUrl { get; set; }
        public string? MembershipAllocatedNumber { get; set; }
        public string? PoBox { get; set; }
        public string? Fax { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CountryId { get; set; }
        public string? RegionId { get; set; }
        public string? TownId { get; set; }
        public bool IsUseOnLineMobileBanking { get; set; }
        public string? MobileOrOnLineBankingLoginState { get; set; }
        public string? CustomerPackageId { get; set; }
        public string? DivisionId { get; set; }
        public string? BranchId { get; set; }
        public string? EconomicActivitiesId { get; set; }
        public string? SignatureUrl { get; set; }
        public string? BankId { get; set; }
        public string? OrganizationId { get; set; }
        public string? Language { get; set; }
        public string? SubDivisionId { get; set; }
        public string? TaxIdentificationNumber { get; set; }
        public bool Active { get; set; }
        public string? EmployerName { get; set; }
        public string? EmployerTelephone { get; set; }
        public string? EmployerAddress { get; set; }
        public string? MaritalStatus { get; set; }
        public string? SpouseName { get; set; }
        public string? SpouseAddress { get; set; }
        public int NumberOfKids { get; set; }
        public string? SpouseOccupation { get; set; }
        public string? SpouseContactNumber { get; set; }
        public double Income { get; set; }
        public string? CustomerCategoryId { get; set; }
        public string? WorkingStatus { get; set; }
        public string? ActiveStatus { get; set; }
    }

}
