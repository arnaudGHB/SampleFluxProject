using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR
{
    public class GetFinancialStatementQuery:IRequest<ServiceResponse<IncomeExpenseStatementDto>>
    {
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
    }
}