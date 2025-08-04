
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new Group.
    /// </summary>
    public class AddGroupCommand : IRequest<ServiceResponse<CreateGroup>>
    {

        public string? GroupName { get; set; }
        public string? MeetingDay { get; set; }
        public DateTime DateOfEstablishment { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GroupCycle { get; set; }

        public string? GroupType { get; set; }
        public string? Email { get; set; }
        public string? CountryId { get; set; }
        public string? RegionId { get; set; }
        public string? TownId { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? HomePhone { get; set; }
        public string? PersonalPhone { get; set; }

        public string? PhotoSource { get; set; }

        public string? Package { get; set; }
        public string? DivisionId { get; set; }
        public string? BranchId { get; set; }

        public string? BankId { get; set; }

        public string? OrganisationId { get; set; }

        public string? SubDivisionId { get; set; }


        public int LoanCycle { get; set; }



    }

}
