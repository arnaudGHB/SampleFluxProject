using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Country.
    /// </summary>
    public class AddCountryCommand : IRequest<ServiceResponse<CountryDto>>
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
