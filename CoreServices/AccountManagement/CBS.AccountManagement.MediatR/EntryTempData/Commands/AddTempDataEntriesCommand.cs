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
    /// <summary>
    /// Represents manual accounting entries
    /// </summary>
    public class AddTempDataEntriesCommand : IRequest<ServiceResponse<bool>>
    {
        public string Description { get; set; }
        public string Reference { get;   set; }
        public bool? IsSyteme { get; set; }
        public string? AccountingEventId { get; set; }
        public string BranchId { get;  set; }
    }
}
