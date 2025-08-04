using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.CashOutThirdPartyP.Commands
{
    /// <summary>
    /// Represents a command to update a DepositLimit.
    /// </summary>
    using System.ComponentModel.DataAnnotations;

    public class CallBackCashOutThirdPartyCommand : IRequest<ServiceResponse<TransactionThirdPartyDto>>
    {
        [Required(ErrorMessage = "Transaction Reference is required.")]
        [StringLength(50, ErrorMessage = "Transaction Reference cannot exceed 50 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "Transaction Reference can only contain letters, numbers, and hyphens.")]
        public string TransactionReference { get; set; }

        [Required(ErrorMessage = "OTP is required.")]
        [Range(1000, 9999, ErrorMessage = "OTP must be a 4-digit number.")]
        public int OTP { get; set; }
    }


}
