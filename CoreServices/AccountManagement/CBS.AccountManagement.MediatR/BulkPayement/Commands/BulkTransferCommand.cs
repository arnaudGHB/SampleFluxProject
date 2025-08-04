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
    public class BulkTransferResultCommand : IRequest<ServiceResponse<BulkResult>>
    {
        public string Id { get; set; }
        public List<Data.BulkTransaction.MakeAccountPostingCommand> MakeAccountPostingCommands { get; internal set; }
        public string MemberName { get; internal set; }
      
        public string MemberReference { get; set; }
        public string TransactionType { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ExternalBranchName { get; set; }
        public string Narration { get; set; }
        public string ExternalBranchcode { get; set; }
        public string TransactionReferenceId { get; set; }
        public string BulkExecutionCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Charges { get; set; }
        public string UserFullName { get; set; }
        public UserInfoToken UserInfoToken { get; set; }
        public string? BranchCode { get; set; }
    }

    public class BulkResult
    {
        public List<Data.AccountingEntry> AccountingEntries { get; set; }
        public List<Data.BulkTransaction.MakeAccountPostingCommand> MakeAccountPostingCommands { get;   set; }
        public string message { get; internal set; }
    }
}
