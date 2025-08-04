using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateDocumentCommand : IRequest<ServiceResponse<DocumentDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
