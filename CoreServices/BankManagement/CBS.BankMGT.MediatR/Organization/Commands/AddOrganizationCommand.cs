using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Organization.
    /// </summary>
    public class AddOrganizationCommand : IRequest<ServiceResponse<OrganizationDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CountryId { get; set; } // Foreign key

    }

}
