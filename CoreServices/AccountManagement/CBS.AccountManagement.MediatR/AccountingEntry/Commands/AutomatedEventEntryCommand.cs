using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AutomatedEventEntryCommand : IRequest<ServiceResponse<EventEntryResponse>>
    {
        public List<AutomatedEventEntry> Entries { get; set; }
        public string ReferenceId { get;  set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string BranchId { get; set; }
    }
    public class EventEntryResponse
    {
        public string ResponseMessge { get; set; }
        public bool Status { get; set; }
 
    }
    public class AutomatedEventEntry
    {
        //
        public string MFI_ChartOfAccountId { get; set; }
        public string BookingDirection { get; set; }

        public decimal Amount { get; set; }
        public string System_Id { get; set; }
    }
}
