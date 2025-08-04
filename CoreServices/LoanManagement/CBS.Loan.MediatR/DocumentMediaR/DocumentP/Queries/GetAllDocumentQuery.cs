using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Queries
{
    public class GetAllDocumentQuery : IRequest<ServiceResponse<List<DocumentDto>>>
    {
    }
}
