using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
 

    public class AddTransferWithdrawalEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReference { get; set; }
        public string EventCode { get; set; }
        public string TransferSource { get; set; }//Teller or Member
        public string ProductId { get; set; }// When transfer is from members account
        public List<TransferCollection> TransferCollection { get; set; }
        public string TransactionReferenceId { get;  set; }
        public DateTime TransactionDate { get; set; }
        public string? MemberReference { get;   set; }
    }

 

}
