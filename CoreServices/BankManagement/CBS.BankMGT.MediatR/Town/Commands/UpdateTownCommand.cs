using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Town.
    /// </summary>
    public class UpdateTownCommand : IRequest<ServiceResponse<TownDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubdivisionId { get; set; } // Foreign key
    }

}
