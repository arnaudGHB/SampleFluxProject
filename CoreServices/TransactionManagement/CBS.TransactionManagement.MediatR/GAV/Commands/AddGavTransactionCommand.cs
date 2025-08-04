using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.GAV.Commands
{

    using System.ComponentModel.DataAnnotations;

    public class AddGavTransactionCommand : IRequest<ServiceResponse<bool>>
    {
        [Required(ErrorMessage = "ApplicationCode is required.")]
        [StringLength(3, ErrorMessage = "ApplicationCode cannot exceed 3 characters.")]
        public string ApplicationCode { get; set; }

        [Required(ErrorMessage = "FromAccount is required.")]
        [StringLength(20, ErrorMessage = "FromAccount cannot exceed 20 characters.")]
        public string FromAccount { get; set; }

        [Required(ErrorMessage = "ToAccount is required.")]
        [StringLength(20, ErrorMessage = "ToAccount cannot exceed 20 characters.")]
        public string ToAccount { get; set; }

        [Required(ErrorMessage = "ReferenceGav is required.")]
        [StringLength(20, ErrorMessage = "ReferenceGav cannot exceed 20 characters.")]
        public string ReferenceGav { get; set; }

        [Required(ErrorMessage = "TransactionDetails are required.")]
        public TransactionDetails Details { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1.0, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "OperationType is required.")]
        [StringLength(20, ErrorMessage = "OperationType cannot exceed 20 characters.")]
        public string OperationType { get; set; }
    }

    public class TransactionDetails
    {
        [Required(ErrorMessage = "Origin is required.")]
        [StringLength(50, ErrorMessage = "Origin cannot exceed 50 characters.")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Msisdn is required.")]
        [StringLength(12, ErrorMessage = "Msisdn must be exactly 12 characters long.")]
        [RegularExpression(@"^237\d{9}$", ErrorMessage = "Msisdn must start with 237 followed by 9 digits.")]
        public string Msisdn { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ref is required.")]
        [StringLength(50, ErrorMessage = "Ref cannot exceed 50 characters.")]
        public string Ref { get; set; }
    }

}
