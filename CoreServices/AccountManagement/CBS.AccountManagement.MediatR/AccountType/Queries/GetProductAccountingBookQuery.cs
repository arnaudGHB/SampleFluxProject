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
    ///// <summary>
    ///// Represents a query to retrieve a specific GetProductAccountingBookQuery by its unique identifier.
    ///// </summary>
    public class GetProductAccountingBookQuery : IRequest<ServiceResponse<List<ProductAccountingBookDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the GetProductAccountingBookQuery to be retrieved.
        /// </summary>
        public string Id { get; set; }


    }
    ///// <summary>
    ///// Represents a query to retrieve a all productAccount by its unique identifier.
    ///// </summary>
    public class GetAllAccountForProductQuery : IRequest<ServiceResponse<List<AccountProduct>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the GetProductAccountingBookQuery to be retrieved.
        /// </summary>
        public string ProductName { get; set; }
    }
    ///// <summary>
    ///// Represents a query to retrieve a all productAccount by its unique identifier.
    ///// </summary>
    public class GetAllAccountForASpecificProductTypeQuery : IRequest<ServiceResponse<List<ProductAccountingChart>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the GetProductAccountingBookQuery to be retrieved.
        /// </summary>
        public string ProductType { get; set; }
    }
}
