using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class CashInitializationCommand : IRequest<ServiceResponse<CashInitResponse>>
    {
        public decimal Amount{ get; set; }
        public bool? CanProceed { get; set; }
        public string TransactionReference { get;  set; }
        public DateTime AccountingDate { get;  set; }
        public string Naration { get;  set; }
        public decimal AmountInVault { get; set; } = 0;
    }

    public class CashInitResponse 
    {
 
        public bool Status { get; set; }
      
        public string Naration { get; set; }

        public CashInitResponse()
        {
                
        }
        public CashInitResponse(bool status,string naration )
        {
            this.Status = status;
            this.Naration = naration;
        }
    }
}