using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Organization.
    /// </summary>
    public class UpdateOrganizationCommand : IRequest<ServiceResponse<OrganizationDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CountryId { get; set; } // Foreign key
    }

}
