using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Account.Queries
{

    /// <summary>
    /// Represents a query to retrieve a specific Account by its BranchID.
    /// </summary>
    public class GetAccountByBranchIDQuery : IRequest<ServiceResponse<AccountDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BranchID to be retrieved.
        /// </summary>
        public string BranchID { get; set; }
    }
}
