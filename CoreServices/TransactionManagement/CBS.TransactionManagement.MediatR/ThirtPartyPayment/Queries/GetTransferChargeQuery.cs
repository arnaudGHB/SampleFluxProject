using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.ThirtPartyPayment.Queries
{
    public class GetTransferChargeQuery : IRequest<ServiceResponse<TransferChargesDto>>
    {
        /// <summary>
        /// The account number of the sender. This field is required, and must be between 6 and 16 characters.
        /// </summary>
        [Required(ErrorMessage = "Sender account number is required.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Sender account number must be between 6 and 16 characters.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Sender account number must contain only numbers.")]
        public string SenderAccountNumber { get; set; }

        /// <summary>
        /// The amount to be transferred. This field is required and must be greater than zero.
        /// </summary>
        [Required(ErrorMessage = "Transfer amount is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Transfer amount must be greater than zero.")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The account number of the receiver. Optional but must be between 6 and 16 characters if provided.
        /// </summary>
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Receiver account number must be between 6 and 16 characters.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Receiver account number must contain only numbers.")]
        public string ReceiverAccountNumber { get; set; }

        /// <summary>
        /// The type of fee operation involved in the transfer. This field is required. 
        /// Example values include: 
        /// - Membership: Registration or member fee.
        /// - Operation: Standard cash transfer.
        /// - CMoney: C-Money transfer.
        /// - Gav: Gav-related transfer.
        /// - MobileMoney: Mobile Money transaction.
        /// - OrangeMoney: Orange Money transaction.
        /// </summary>
        [Required(ErrorMessage = "Fee operation type is required.")]
        public string FeeOperationType { get; set; }
    }


}

