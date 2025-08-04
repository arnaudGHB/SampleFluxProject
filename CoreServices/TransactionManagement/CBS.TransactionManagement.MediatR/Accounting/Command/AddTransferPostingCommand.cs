using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class AddTransferPostingCommand : IRequest<ServiceResponse<bool>>
    {

        public string FromProductId { get; set; }
        public string ToProductId { get; set; }
        public string FromMemberReference { get; set; }
        public string TransactionReferenceId { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public string ExternalBranchId { get; set; }
        public string ExternalBranchCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<AmountCollectionItem> AmountCollection { get; set; }
        public AddTransferPostingCommand()
        {
            AmountCollection = new List<AmountCollectionItem>();
        }

    }



    //Database TSCDatabase

    public class AmountCollectionItem
    {
        public string EventAttributeName { get; set; }
        public string LiaisonEventName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPrincipal { get; set; }
        public string Naration { get; set; }
        public string WhichSourceAccountPaysTheCharges { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AmountCollectionItem class.
        /// </summary>
        /// <param name="isNone">Set to true if no account pays the charges.</param>
        /// <param name="isSourceAccountToPay">Set to true if the source account pays the charges.</param>
        /// <param name="isDestinationAccountToPay">Set to true if the destination account pays the charges.</param>
        public AmountCollectionItem(bool isNone, bool isSourceAccountToPay, bool isDestinationAccountToPay)
        {
            if (isNone)
            {
                this.WhichSourceAccountPaysTheCharges = "None";
            }
            else if (isDestinationAccountToPay)
            {
                this.WhichSourceAccountPaysTheCharges = "Destination";
            }
            else
            {
                this.WhichSourceAccountPaysTheCharges = "Source";
            }
        }


    }


}
