using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanApplicationCommand : IRequest<ServiceResponse<LoanApplicationDto>>
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public string CustomerId { get; set; } // Member Reference ID

        [Required(ErrorMessage = "Loan amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Loan amount must be greater than zero.")]
        public decimal Amount { get; set; } // Loan amount applied

        [Required(ErrorMessage = "Loan duration is required.")]
        [Range(1, 360, ErrorMessage = "Loan duration must be between 1 and 360 months.")]
        public int LoanDuration { get; set; } // Loan term in months

        [Required(ErrorMessage = "Interest rate is required.")]
        [Range(0, 100, ErrorMessage = "Interest rate must be between 0% and 100%.")]
        public decimal InterestRate { get; set; } // Loan interest rate

        public bool RequiredDownPaymentCoverageRate { get; set; } // Determines if a down payment is required

        public bool IsThereGuarantor { get; set; } // Determines if a guarantor is provided

        public bool IsThereCollateral { get; set; } // Determines if a collateral is provided

        [Range(0, double.MaxValue, ErrorMessage = "Ordinary share account coverage amount must be a non-negative value.")]
        public decimal ShareAccountCoverageAmount { get; set; } // Ordinary shares used for coverage

        [Range(0, 100, ErrorMessage = "Saving account coverage rate must be between 0% and 100%.")]
        public decimal SavingAccountCoverageRate { get; set; } // Saving coverage percentage

        [Required(ErrorMessage = "At least one fee must be selected.")]
        [MinLength(1, ErrorMessage = "Select at least one loan fee.")]
        public List<string> FeeIds { get; set; } // Selected Loan Fees for this loan application

        public UpdateLoanApplicationCommand()
        {
            FeeIds = new List<string>();
        }
    }

}
