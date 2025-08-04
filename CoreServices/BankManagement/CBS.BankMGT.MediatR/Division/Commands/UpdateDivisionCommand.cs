using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Division.
    /// </summary>
    public class UpdateDivisionCommand : IRequest<ServiceResponse<DivisionDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RegionId { get; set; } // Foreign key
    }

}
