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


    public class UpdateBudgetAdjustmentCommand : IRequest<ServiceResponse<BudgetAdjustmentDto>>
    {

        public string Id { get; set; }
        public string BudgetPlanId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }

   


}
