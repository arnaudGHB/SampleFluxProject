using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Queries
{
    public class GetAllDocumentAttachedToLoanQuery : IRequest<ServiceResponse<List<DocumentAttachedToLoanDto>>>
    {
    }
}
