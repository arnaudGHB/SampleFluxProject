using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to ReverseAccountingEntry.
    /// </summary>
    public class ReverseAccountingEntryCommand : IRequest<ServiceResponse<bool>>
    {
        public string  ReferenceId { get; set; }


    }

     
}