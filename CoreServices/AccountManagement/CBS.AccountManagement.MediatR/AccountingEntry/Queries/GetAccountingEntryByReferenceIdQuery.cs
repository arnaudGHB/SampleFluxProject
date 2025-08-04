using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class GetAccountingEntryByReferenceIdQuery : IRequest<CBS.AccountManagement.Helper.ServiceResponse<System.Collections.Generic.List<CBS.AccountManagement.Data.AccountingEntryDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountingEntry to be retrieved.
        /// </summary>
        public string? ReferenceId { get; set; }
        public GetAccountingEntryByReferenceIdQuery()
        {
                
        }
        public GetAccountingEntryByReferenceIdQuery(string referenceID)
        {
            ReferenceId=referenceID;
        }
    }
}
