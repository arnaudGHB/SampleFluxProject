using Microsoft.AspNetCore.Http;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands
{
    public class AddDocumentAttachedToLoanCommand
    {
        public string LoanApplicationID { get; set; }
        public IFormFileCollection AttachedFiles { get; set; }
    }
}
