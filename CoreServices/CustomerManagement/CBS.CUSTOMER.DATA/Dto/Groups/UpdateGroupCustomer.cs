
using CBS.CUSTOMER.HELPER.Helper;


namespace CBS.CUSTOMER.DATA.Dto
{
    public class UpdateGroupCustomer
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Citizenship { get; set; }


        public string? Gender { get; set; } = GenderType.Male.ToString();

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? IDNumber { get; set; }


        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? Town { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? HomePhone { get; set; }
        public string? PersonalPhone { get; set; }
        public string? PhotoSource { get; set; }

        public bool IsUseOnlineMobileBanking { get; set; }

        public string? Package { get; set; }
        public string? Division { get; set; }
        public string? BranchId { get; set; }
        public string? EconomicActivitesId { get; set; }
        public string? BankId { get; set; }

        public string? OrganisationId { get; set; }

        public bool Active { get; set; }

        public string? GroupId { get; set; }
        public string? UserId { get; set; }

    }
}
