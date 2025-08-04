using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddDepositNotificationCommand : IRequest<ServiceResponse<bool>>
    {
        public string? BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }
}
