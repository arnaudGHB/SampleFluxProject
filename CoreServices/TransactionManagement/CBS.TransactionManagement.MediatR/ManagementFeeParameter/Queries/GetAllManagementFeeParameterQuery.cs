using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllManagementFeeParameterQuery : IRequest<ServiceResponse<List<ManagementFeeParameterDto>>>
    {
    }
}
