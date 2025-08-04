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
    public class GetOperationEventAttributeByOperationAccountTypeIdQuery : IRequest<ServiceResponse<List<OperationEventAttributesDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountType to be retrieved.
        /// </summary>
        public string OperationAccountTypeId { get; set; }

    }
}
