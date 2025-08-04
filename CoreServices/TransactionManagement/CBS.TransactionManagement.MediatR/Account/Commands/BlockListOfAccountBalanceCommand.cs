using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.Commands
{
    public class BlockListOfAccountBalanceCommand : IRequest<ServiceResponse<bool>>
    {
        public string LoanApplicationId { get; set; }
        public List<AccountToBlocked> AccountToBlockeds { get; set; }
        public bool IsRelease { get; set; }
    }
    public class AccountToBlocked
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
}
