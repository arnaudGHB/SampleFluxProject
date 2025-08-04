using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR
{
    public class GetProfitAndLossStatementQuery:IRequest<ServiceResponse<IncomeExpenseStatementDto>>
    {
        public DateTime Date { get; set; }
        public string? QueryLevel { get; set; }
        public  string ? BranchId { get; set; }
    }
}