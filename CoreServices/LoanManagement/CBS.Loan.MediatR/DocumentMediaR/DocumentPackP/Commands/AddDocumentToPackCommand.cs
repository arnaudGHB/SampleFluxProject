using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddDocumentToPackCommand : IRequest<ServiceResponse<DocumentJoinDto>>
    {
        public string DocumentPackId { get; set; }
        public string DocumentId { get; set; }
    }

}
