using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Region.
    /// </summary>
    public class AddRegionCommand : IRequest<ServiceResponse<RegionDto>>
    {
        public string CountryId { get; set; }
        public string Name { get; set; }
    }

}
