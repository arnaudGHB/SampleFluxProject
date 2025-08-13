using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands
{
    public class DeleteStandingOrderCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
    }


}
