using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Currency.
    /// </summary>
    public class AddCurrencyCommand : IRequest<ServiceResponse<CurrencyDto>>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryID { get; set; }
    }

}
