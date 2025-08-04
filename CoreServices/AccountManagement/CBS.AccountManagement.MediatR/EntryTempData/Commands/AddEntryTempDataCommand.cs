using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class AddEntryTempDataCommand : IRequest<ServiceResponse<EntryTempDataDto>>
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public string BookingDirection { get; set; }
   
        public decimal Amount { get; set; }
        public string AccountBalance { get; set; }
        public string Description { get; set; }
        public string PositingSource { get; set; }
        public bool? IsdoubbleValidationRequired { get; set; }
        public string? AccountingEventId { get; set; } = "MANUAL USER";
        public string Reference { get; set; }
        public string BranchId { get; set; }
        public string ExternalBranchId { get;  set; }
    }
    public class AddEntryTempDataListCommand : IRequest<ServiceResponse<bool>>
    {
        public List<EntryTempDataCommand> EntryTempDatas { get; set; }

        public bool IsDoubleValidationNeeded { get; set; }
        public bool IsSystem { get; set; }
        public string BranchId { get; set; }
        public string?   AccountingEventRuleId { get; set; }
        public List<string>? ListOfBranchIds { get; set; }
        public string? ExternalBranchId { get;   set; }
        public bool? IsInterBranchTransaction { get; set; }
    }
    public class EntryTempDataCommand
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public string BookingDirection { get; set; }

        public decimal Amount { get; set; }
        public string AccountBalance { get; set; }
        public string Description { get; set; }

        public string Reference { get; set; }
        public string? AccountingEventId { get; set; }
        public string BranchId { get; set; }
        public string ExternalBranchId { get; set; }
    }

    }