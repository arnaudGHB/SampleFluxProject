using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.otherCashIn.Commands
{
    /// <summary>
    /// Represents a command to add a new DepositLimit.
    /// </summary>
    using System.ComponentModel.DataAnnotations;

    public class AddOtherTransactionMobileMoneyCommand : IRequest<ServiceResponse<OtherTransactionDto>>
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "None Customer Name is required.")]
        [StringLength(100, ErrorMessage = "Customer Name cannot be longer than 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Operator Type is required.")]
        [RegularExpression("MobileMoneyMTN|MobileMoneyORANGE", ErrorMessage = "Operator Type must be either 'Mobile Money MTN' or 'Mobile Money Orange'.")]
        public string SourceType { get; set; }

        [Required(ErrorMessage = "National identity card is required.")]
        [StringLength(30, ErrorMessage = "National identity card cannot be longer than 30 characters.")]
        public string CNI { get; set; }

        [Required(ErrorMessage = "Telephone Number is required.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Telephone Number must be a valid 9-digit number.")]
        public string TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Booking Direction is required.")]
        [RegularExpression("Deposit|Withdrawal", ErrorMessage = "Booking Direction must be either 'Deposit' or 'Withdrawal'.")]
        public string BookingDirection { get; set; }

        [Required(ErrorMessage = "Operation Type is required.")]
        [StringLength(50, ErrorMessage = "Operation Type cannot be longer than 50 characters.")]
        public string OperationType { get; set; }

        [Required(ErrorMessage = "Cash Operation status is required.")]
        public bool IsCashOperation { get; set; }

        [Required(ErrorMessage = "None Member Reference is required.")]
        [StringLength(11, ErrorMessage = "Member Reference cannot be longer than 10 characters.")]
        public string MemberReference { get; set; }
        public string TellerCode { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }
    

}
