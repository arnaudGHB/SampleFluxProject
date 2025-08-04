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
    /// Represents a query to retrieve a specific Account of a given person by its referenceId.
    /// </summary>
    public class GetAccountByReferenceQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string referenceId { get; set; }
    }
}
