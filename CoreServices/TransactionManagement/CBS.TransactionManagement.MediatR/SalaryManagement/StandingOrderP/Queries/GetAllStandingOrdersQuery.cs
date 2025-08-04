using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries
{
    public class GetAllStandingOrdersQuery : IRequest<ServiceResponse<List<StandingOrderDto>>>
    {
        public string? BranchId { get; set; }
    }
}
