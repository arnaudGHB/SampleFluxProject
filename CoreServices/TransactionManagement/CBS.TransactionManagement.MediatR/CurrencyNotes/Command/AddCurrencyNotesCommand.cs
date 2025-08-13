using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new CurrencyNotes.
    /// </summary>
    public class AddCurrencyNotesCommand : IRequest<ServiceResponse<List<CurrencyNotesDto>>>
    {
        public CurrencyNotesRequest CurrencyNote;
        public string Reference { get; set; }
        public string? ServiceType { get; set; }

    }

}
