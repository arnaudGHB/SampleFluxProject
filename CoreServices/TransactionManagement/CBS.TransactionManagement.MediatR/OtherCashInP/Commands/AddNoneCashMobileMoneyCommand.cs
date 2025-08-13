using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.otherCashIn.Commands
{
    public class AddNoneCashMobileMoneyCommand : IRequest<ServiceResponse<OtherTransactionDto>>
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(100, ErrorMessage = "Customer Name cannot be longer than 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Operator Type is required.")]
        [RegularExpression("MobileMoneyMTN|MobileMoneyORANGE", ErrorMessage = "Operator Type must be either 'MobileMoneyMTN' or 'MobileMoneyORANGE'.")]
        public string SourceType { get; set; }

        [Required(ErrorMessage = "Telephone Number is required.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Telephone Number must be a valid 9-digit number.")]
        public string TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Operation Type is required.")]
        [StringLength(50, ErrorMessage = "Operation Type cannot be longer than 50 characters.")]
        public string OperationType { get; set; }

        [Required(ErrorMessage = "Member Reference is required.")]
        [StringLength(10, ErrorMessage = "Member Reference cannot be longer than 10 characters.")]
        public string MemberReference { get; set; }

        [StringLength(20, ErrorMessage = "Teller Code cannot be longer than 20 characters.")]
        public string TellerCode { get; set; }

        [StringLength(20, ErrorMessage = "Receiver Account Number cannot be longer than 20 characters.")]
        public string ReceiverAccountNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Charges must be greater than or equal to zero.")]
        public decimal Charges { get; set; }
    }


}
