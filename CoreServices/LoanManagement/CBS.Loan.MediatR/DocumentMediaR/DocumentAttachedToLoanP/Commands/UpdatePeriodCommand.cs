using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateDocumentAttachedToLoanCommand : IRequest<ServiceResponse<DocumentAttachedToLoanDto>>
    {
        public string Id { get; set; }
        public string LoanApplicationID { get; set; }
        public IFormFileCollection AttachedFiles { get; set; }
    }

}
