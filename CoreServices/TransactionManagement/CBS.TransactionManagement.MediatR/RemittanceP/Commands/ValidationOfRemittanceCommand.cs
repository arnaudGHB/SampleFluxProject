using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Commands
{
    /// <summary>
    /// Represents a command to validate a remittance.
    /// </summary>
    public class ValidationOfRemittanceCommand : IRequest<ServiceResponse<RemittanceDto>>
    {
        [Required(ErrorMessage = "The Id field is mandatory and cannot be empty.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "The Status field is mandatory and cannot be empty.")]
        public string Status { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "The Approval Comment must be between 10 and 500 characters in length.")]
        public string ApprovalComment { get; set; }
    }


}
