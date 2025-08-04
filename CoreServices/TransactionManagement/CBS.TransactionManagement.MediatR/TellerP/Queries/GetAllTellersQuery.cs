using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Queries
{
    public class GetAllTellerQuery : IRequest<ServiceResponse<List<TellerDto>>>
    {
    }
}
