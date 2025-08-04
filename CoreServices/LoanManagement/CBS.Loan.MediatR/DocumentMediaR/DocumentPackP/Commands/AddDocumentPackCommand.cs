using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddDocumentPackCommand : IRequest<ServiceResponse<DocumentPackDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> DocumentIds { get; set; }
    }

}
