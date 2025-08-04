
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class GetAllCustomers
    {

        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsMemberOfACompany { get; set; }
        public string? VillageOfOrigin { get; set; }
        public bool IsBelongToGroup { get; set; }
        public bool? IsMemberOfAGroup { get; set; }
        public string? Matricule { get; set; }
        public string? AccountConfirmationNumber { get; set; }

        public string? LegalForm { get; set; }
        public string? FormalOrInformalSector { get; set; }
        public string? BankingRelationship { get; set; } //This is an enum//  1.  CB-Customer  2. NCB-Non-customer
        public DateTime DateOfBirth { get; set; }
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
        public string? POBox { get; set; }
        public string? Fax { get; set; }
        public string? Gender { get; set; } = GenderType.Male.ToString();
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CountryId { get; set; }
        public string? RegionId { get; set; }
        public string? TownId { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsUseOnLineMobileBanking { get; set; }
        public string? MobileOrOnLineBankingLoginState { get; set; }
        public string? CustomerPackageId { get; set; }

        public string? CustomerCode { get; set; }
        public string? BankName { get; set; }
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
        public string? SecretQuestion { get; set; }
        public string? SecretAnswer { get; set; }
        public string? EmployerName { get; set; }
        public string? EmployerTelephone { get; set; }
        public string? EmployerAddress { get; set; }
        public string? MaritalStatus { get; set; }
        public string? SpouseName { get; set; }
        public string? SpouseAddress { get; set; }
        public int? NumberOfKids { get; set; }
        public string? SpouseOccupation { get; set; }
        public string? SpouseContactNumber { get; set; }
        public double? Income { get; set; }
        public string? CustomerCategoryId { get; set; }
        public string? WorkingStatus { get; set; }
        public string? ActiveStatus { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime CompanyCreationDate { get; set; }
        public string? PlaceOfCreation { get; set; }
        public string? ConditionForWithdrawal { get; set; }

        public PaginationMetadata PaginationMetadata { get; set; }

    }
    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int TotalPages { get; set; }
    }
    public class CustomerStatisticsDto
    {




        public int TotalNumberOfPhysical { get; set; }
        public int TotalNumberOfMoral { get; set; }
        public int TotalBranches { get; set; }
        public int TotalMembers { get; set; }
        public List<CustomerStatisticsByBranchDto> CustomerStatisticsByBranches { get; set; }
    }
    public class CustomerStatisticsByBranchDto
    {
        public int NumberOfPhysical { get; set; }
        public int NumberOfMoral { get; set; }
        public int NumberOfMembers { get; set; }
        public string BranchId { get; set; }
    }


    public class CustomerListingDto
    {

        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string BankingRelationship { get; set; } //This is an enum//  1.  CB-Customer  2. NCB-Non-customer
        public DateTime RegistrationDate { get; set; }
        public string IDNumber { get; set; }
        public string IDNumberIssueDate { get; set; }
        public string MembershipApprovalStatus { get; set; }
        public string Address { get; set; }
        public string BranchId { get; set; }
        public string PhoneNumber { get; set; }
        public string Matricule { get; set; }
        public string? AccountConfirmationNumber { get; set; }
        public string Phone { get; set; }

    }

}
