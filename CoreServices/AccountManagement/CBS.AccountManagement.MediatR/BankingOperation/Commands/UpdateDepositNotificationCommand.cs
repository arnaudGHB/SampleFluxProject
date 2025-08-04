using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UpdateDepositNotificationCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string BankTransactionReference { get; set; }
        public string FilePath { get; set; }
        public string Comment { get; set; }
        public string ValueDate { get;  set; }
  
    }
}
