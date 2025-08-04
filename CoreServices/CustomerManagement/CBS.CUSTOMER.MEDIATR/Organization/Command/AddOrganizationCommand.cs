
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Organization.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new Organization.
    /// </summary>
    public class AddOrganizationCommand : IRequest<ServiceResponse<CreateOrganization>>
    {

        public string? OrganizationName { get; set; }
        public string? ShortName { get; set; }
        public int OrganizationCycle { get; set; }

        public string? Email { get; set; }
        public string? CountryId { get; set; }
        public string? RegionId { get; set; }
        public string? TownId { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? EconomicActivityId { get; set; }
        public string? OrganisationIndentificationNumber { get; set; }
        public string? LegalForm { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NumberOfVolunteers { get; set; }
        public int FiscalStatus { get; set; }
        public int Affilation { get; set; }


        public string? PhotoSource { get; set; }

        public string? Package { get; set; }
        public string? DivisionId { get; set; }


        public string? SubDivisionId { get; set; }

    }

}
