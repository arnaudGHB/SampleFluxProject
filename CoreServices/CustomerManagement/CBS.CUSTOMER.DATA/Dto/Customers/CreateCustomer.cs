


using CBS.CUSTOMER.HELPER.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateCustomer 
    {
        public string? RegistrationNumber { get; set; }
        public DateTime CompanyCreationDate { get; set; }
        public string? PlaceOfCreation { get; set; }
        public string? ConditionForWithdrawal { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FormalOrInformalSector { get; set; }
        public string? Gender { get; set; } = GenderType.Male.ToString();
        public string VillageOfOrigin { get; set; }
        public string? MatriculeNumber { get; set; }
        public string? AccountConfirmationNumber { get; set; }
        public string MobileLoginId { get; set; }
        public string DefaultPhoneNumber { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? IDNumber { get; set; }
        public string? CountryId { get; set; }
        public string? RegionId { get; set; }
        public string? TownId { get; set; }
        public string? Address { get; set; }
        public string? BankName { get; set; }
        public string? PhotoSource { get; set; }
   
        public bool IsUseOnLineMobileBanking { get; set; }
        public string? PackageId { get; set; }
        public string? DivisionId { get; set; }
        public string? BranchId { get; set; }

        public string? CustomerCode { get; set; }
        public string? EconomicActivitiesId { get; set; }
        public string? BankId { get; set; }
        public string? Pin { get; set; }

        public string? OrganizationId { get; set; }

        public string? SubDivisionId { get; set; }

        public string? TaxIdentificationNumber { get; set; }

        public string? CustomerId { get; set; }

        public bool? IsMemberOfACompany { get; set; }
        public bool? IsMemberOfAGroup { get; set; }
        public string? LegalForm { get; set; }
        public string? BankingRelationship { get; set; }

        public string? PlaceOfBirth { get; set; }
        public string? Occupation { get; set; }
        public string? IDNumberIssueDate { get; set; }
        public string? IDNumberIssueAt { get; set; }

        public string? MembershipApprovalStatus { get; set; }

        public string? POBox { get; set; }
        public string? Fax { get; set; }
   
        public string? CustomerPackageId { get; set; }
   
    
        public string? Language { get; set; }
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

    }
    public class CreateMemberOrdinaryAccount
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
