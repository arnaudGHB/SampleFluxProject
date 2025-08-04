using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class CashClearingTransferFromBankDepositCommand : IRequest<ServiceResponse<bool>>
    {
        public string ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public string ToAccountId { get; set; }
        public string FromAccountId { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }
}
