using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Organisation.
    /// </summary>
    public class UpdateOrganizationCommand : IRequest<ServiceResponse<UpdateOrganization>>
    {
        public string? Id { get; set; }
        public string? OrganisationName { get; set; }
        public string? ShortName { get; set; }
        public int OrganisationCycle { get; set; }

        public string? Email { get; set; }
        public string? CountryId { get; set; }
        public string? RegionId { get; set; }
        public string? DivisionId { get; set; }
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


        public string? SubdivisionId { get; set; }
        public bool Active { get; set; }
      

    }

}
