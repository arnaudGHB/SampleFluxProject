using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Subdivision.
    /// </summary>
    public class UpdateSubdivisionCommand : IRequest<ServiceResponse<SubdivisionDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DivisionId { get; set; } // Foreign key
    }

}
