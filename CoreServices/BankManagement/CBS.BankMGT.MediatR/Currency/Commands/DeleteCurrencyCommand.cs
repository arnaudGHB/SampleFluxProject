using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Currency.
    /// </summary>
    public class DeleteCurrencyCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Currency to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
