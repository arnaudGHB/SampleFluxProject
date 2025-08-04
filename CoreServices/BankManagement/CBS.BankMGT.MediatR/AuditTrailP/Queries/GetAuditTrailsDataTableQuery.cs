using CBS.BankMGT.Data.Pagging;
using CBS.BankMGT.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.MediatR.AuditTrailP.Queries
{
    public class GetAuditTrailsDataTableQuery : IRequest<ServiceResponse<CustomDataTable>>
    {
        public DataTableOptions DataTableOptions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Feild { get; set; }
    }

}
