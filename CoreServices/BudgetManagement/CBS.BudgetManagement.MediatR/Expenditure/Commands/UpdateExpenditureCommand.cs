using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
namespace CBS.BudgetManagement.MediatR.Commands
{
    // Asset Commands 


    public class UpdateExpenditureCommand : IRequest<ServiceResponse<ExpenditureDto>>
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string BranchId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string BudgetItemId { get; set; }
    }

   


}
