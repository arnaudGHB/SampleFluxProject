using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Region.
    /// </summary>
    public class UpdateRegionCommand : IRequest<ServiceResponse<RegionDto>>
    {
        public string Id { get; set; }
        public string CountryId { get; set; }
        public string Name { get; set; }
    }

}
