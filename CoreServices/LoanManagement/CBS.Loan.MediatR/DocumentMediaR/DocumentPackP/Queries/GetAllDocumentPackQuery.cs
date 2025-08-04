using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Queries
{
    public class GetAllDocumentPackQuery : IRequest<ServiceResponse<List<DocumentPackDto>>>
    {
    }
}
