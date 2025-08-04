using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Country.
    /// </summary>
    public class UpdateCountryCommand : IRequest<ServiceResponse<CountryDto>>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
