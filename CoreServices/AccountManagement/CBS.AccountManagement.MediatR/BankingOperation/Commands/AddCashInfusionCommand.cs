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
    public class AddCashInfusionCommand : IRequest<ServiceResponse<bool>>
    {
        public decimal Amount { get; set; }
        public string RequestMessage { get; set; }
      
        //public string CurrentOpenOfDayHistoryId { get; set; }
    }
}
