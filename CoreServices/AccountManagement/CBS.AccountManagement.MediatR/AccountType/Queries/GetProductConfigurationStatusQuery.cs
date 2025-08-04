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
    /// Represents a query to retrieve a specific AccountType by its unique identifier.
    /// </summary>
    public class GetProductConfigurationStatusQuery : IRequest<ServiceResponse<List<ProductAccountConfiguration>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountType to be retrieved.
        /// </summary>
        public string Id { get; set; }

        public string IdType { get; set; }
    }

 
    public class GetAllProductConfigurationStatusQuery : IRequest<ServiceResponse<List<ProductAccountConfiguration>>>
    {
         
 
    }
}
