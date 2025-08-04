using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a EconomicActivity.
    /// </summary>
    public class UpdateEconomicActivityCommand : IRequest<ServiceResponse<EconomicActivityDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
