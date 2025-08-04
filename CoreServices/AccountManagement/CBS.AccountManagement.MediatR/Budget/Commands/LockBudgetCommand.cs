using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Budget.Commands
{ 
    public class LockBudgetCommand : IRequest<ServiceResponse<BudgetDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BudgetApproval to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
