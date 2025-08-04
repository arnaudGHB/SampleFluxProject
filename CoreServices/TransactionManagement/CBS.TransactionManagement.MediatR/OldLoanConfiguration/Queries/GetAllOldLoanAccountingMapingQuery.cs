using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.OldLoanConfiguration.Queries
{
    public class GetAllOldLoanAccountingMapingQuery : IRequest<ServiceResponse<List<OldLoanAccountingMapingDto>>>
    {
    }
}
