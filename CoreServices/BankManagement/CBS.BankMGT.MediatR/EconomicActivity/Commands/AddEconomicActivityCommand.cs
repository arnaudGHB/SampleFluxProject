using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new EconomicActivity.
    /// </summary>
    public class AddEconomicActivityCommand : IRequest<ServiceResponse<EconomicActivityDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
