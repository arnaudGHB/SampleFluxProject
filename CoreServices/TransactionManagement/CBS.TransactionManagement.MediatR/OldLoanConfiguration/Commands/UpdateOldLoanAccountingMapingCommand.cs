using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.OldLoanConfiguration.Commands
{
    /// <summary>
    /// Represents a command to update a CloseFeeParameter.
    /// </summary>
    public class UpdateOldLoanAccountingMapingCommand : IRequest<ServiceResponse<OldLoanAccountingMapingDto>>
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Loan Type Name is required.")]
        [StringLength(50, ErrorMessage = "Loan Type Name cannot exceed 50 characters.")]
        public string LoanTypeName { get; set; }

        [Required(ErrorMessage = "GL for VAT is required.")]
        [StringLength(36, ErrorMessage = "GL for VAT must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for VAT must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForVAT { get; set; }

        [Required(ErrorMessage = "GL for Interest is required.")]
        [StringLength(36, ErrorMessage = "GL for Interest must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for Interest must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForInterest { get; set; }

        [Required(ErrorMessage = "GL for Capital is required.")]
        [StringLength(36, ErrorMessage = "GL for Capital must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for Capital must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForCapital { get; set; }

        [StringLength(36, ErrorMessage = "GL for Provision More Than One Year must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for Provision More Than One Year must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForProvisionMoreThanOneYear { get; set; }

        [StringLength(36, ErrorMessage = "GL for Provision More Than Two Years must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for Provision More Than Two Years must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForProvisionMoreThanTwoYear { get; set; }

        [StringLength(36, ErrorMessage = "GL for Provision More Than Three Years must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for Provision More Than Three Years must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForProvisionMoreThanThreeYear { get; set; }

        [StringLength(36, ErrorMessage = "GL for Provision More Than Four Years must not exceed 36 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "GL for Provision More Than Four Years must contain only alphanumeric characters and dashes.")]
        public string ChartOfAccountIdForProvisionMoreThanFourYear { get; set; }
    }

}
