using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands
{
    public class AddLoanAttachedDocumentCallBackCommand: IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string UrlPath { get; set; }
        public string DocumentName { get; set; }
        public string Extension { get; set; }
        public string BaseUrl { get; set; }
        public string DocumentType { get; set; }
        public string DocumentId { get; set; }
    }
}
