using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.ChangeManagement.Command
{
    /// <summary>
    /// Command for managing vault cash changes.
    /// </summary>
    public class VaultCashChangeCommand : IRequest<ServiceResponse<CashChangeHistoryDto>>
    {
        public CurrencyNotesRequest DenominationsGiven { get; set; }
        public CurrencyNotesRequest DenominationsReceived { get; set; }
        public string ChangeReason { get; set; }
    }
}
