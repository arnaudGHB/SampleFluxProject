using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{

    /// <summary>
    /// Represents a command to delete a SavingProduct.
    /// </summary>
    public class DeleteSavingProductCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the SavingProduct to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
