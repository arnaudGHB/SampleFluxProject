using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Subdivision.
    /// </summary>
    public class AddSubdivisionCommand : IRequest<ServiceResponse<SubdivisionDto>>
    {

        public string Name { get; set; }
        public string DivisionId { get; set; } 

    }

}
