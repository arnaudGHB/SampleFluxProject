using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Queries
{

    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>
    public class GetAccountCategoryByAccountNumberQuery : IRequest<ServiceResponse<List<AccountClassCategoryDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary> 
        public string AccountNumber { get;   set; }
    }
}
