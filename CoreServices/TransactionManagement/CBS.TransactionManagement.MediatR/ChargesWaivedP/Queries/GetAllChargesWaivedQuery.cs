using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.ChargesWaivedP
{
    public class GetAllChargesWaivedQuery : IRequest<ServiceResponse<List<ChargesWaivedDto>>>
    {
    }
}
