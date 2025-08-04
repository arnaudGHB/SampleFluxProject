using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAccountGLQuery : IRequest<ServiceResponse<AccountingGeneralLedger>>
    {
        public DateTime ToDate { get; set; }


        public DateTime FromDate { get; set; }

        public string? SearchOption { get; set; }

        public string? BranchId { get; set; }

        public string? AccountId { get; set; }

    
    }
    public class GetListAccountGLQuery :  IRequest<ServiceResponse<AccountLedgerDetails>>
    {
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string? BranchId { get; set; }
        public List<string>? AccountIds { get; set; }
    }
    public class GetJournalEntryQuery : IRequest<ServiceResponse<AccountingEntriesReport>>
    {
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string BranchId { get; set; }

    }
}
