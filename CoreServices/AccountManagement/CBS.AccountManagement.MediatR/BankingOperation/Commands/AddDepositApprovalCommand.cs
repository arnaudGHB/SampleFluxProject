using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Helper;
using MediatR;
namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddDepositApprovalCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string? BankAccountId { get; set; }
        public string? CorrepondingBranchId { get; set; }
        public string? Status { get; set; }
        public string ApprovedMessage { get;  set; }
   
    }
}
