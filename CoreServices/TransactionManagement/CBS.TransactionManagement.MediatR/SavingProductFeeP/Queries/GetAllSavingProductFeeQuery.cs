using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Queries
{
    public class GetAllSavingProductFeeQuery : IRequest<ServiceResponse<List<SavingProductFeeDto>>>
    {
    }
}
