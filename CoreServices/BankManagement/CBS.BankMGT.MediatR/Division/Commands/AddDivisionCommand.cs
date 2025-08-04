using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Division.
    /// </summary>
    public class AddDivisionCommand : IRequest<ServiceResponse<DivisionDto>>
    {
   
        public string Name { get; set; }
        public string RegionId { get; set; } 

    }

}
