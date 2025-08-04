using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.BankingOperation.Commands
{
    public class AddCashReplenishmentApprovalCommand : IRequest<ServiceResponse<bool>>
    {

        public string Id { get; set; }
       public string CashRequisitionType { get; set; }
        public string CorrespondingBranchId { get; set; }
        public string ApprovedMessage { get; set; }
        public string Status { get; set; }
        public decimal ApprovedAmount { get; set; }
        public bool IsApproved { get; set; }
        public string BranchCode { get;  set; }
        public string? AccountId { get;  set; }
    }
}
