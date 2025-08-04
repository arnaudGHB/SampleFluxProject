using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands
{
    public class DocumentAttachedToLoanCommand : IRequest<ServiceResponse<DocumentAttachedToLoanDto>>
    {

        public IFormFileCollection AttachedFiles { get; set; }
        public string RootPath { get; set; }
        public string BaseURL { get; set; }
        public string LoanApplicationId { get; set; }
        public string DocumentId { get; set; }
    }
}
//AddLoanAttachedDocumentCallBackCommand