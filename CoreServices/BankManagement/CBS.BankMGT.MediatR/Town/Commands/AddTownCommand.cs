using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Town.
    /// </summary>
    public class AddTownCommand : IRequest<ServiceResponse<TownDto>>
    {
   
        public string Name { get; set; }
        public string RegionId { get; set; } 

    }

}
