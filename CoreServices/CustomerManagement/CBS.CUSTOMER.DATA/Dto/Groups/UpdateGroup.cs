

namespace CBS.CUSTOMER.DATA.Dto
{
    public class UpdateGroup 
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupTypeId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxPayerNumber { get; set; }
        public string DateOfEstablishment { get; set; }
        public string Email { get; set; }
        public string CountryId { get; set; }
        public string RegionId { get; set; }
        public string TownId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PhotoSource { get; set; }
        public string Package { get; set; }
        public string DivisionId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string SubDivisionId { get; set; }
        public bool Active { get; set; } = false;
        public string? BankCode { get; set; }
        public string? BranchCode { get; set; }
        public string EconomicActivitiesId { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Occupation { get; set; }
        public string? IDNumberIssueDate { get; set; }
        public string? IDNumberIssueAt { get; set; }
        public string? IDNumber { get; set; }
        public string? MembershipApprovalStatus { get; set; }
        public double? Income { get; set; } = 0;
        public string? POBox { get; set; }
        public string? Fax { get; set; }
        public string? WorkingStatus { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string CustomerCategoryId { get; set; }

    }
}
