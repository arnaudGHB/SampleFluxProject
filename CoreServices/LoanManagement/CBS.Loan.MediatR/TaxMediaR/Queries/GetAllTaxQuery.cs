using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.TaxMediaR.Queries
{
    public class GetAllTaxQuery : IRequest<ServiceResponse<List<TaxDto>>>
    {
    }
}
