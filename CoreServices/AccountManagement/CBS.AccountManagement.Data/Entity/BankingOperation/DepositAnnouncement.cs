using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
  
    public class DepositNotification:BaseEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }

        public string ApprovalKey { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public bool IsApproved { get; set; }
        public bool HasBankAccount { get; set; }

        public string BankAccountId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime DepositDate { get; set; }
        public string ApprovedMessage { get; set; }
        public string? CorrepondingBranchId { get; set; }

        public static DepositNotification SetDepositCommand(string stringData, UserInfoToken userInfoToken)
        {
            DepositNotification command = JsonConvert.DeserializeObject<DepositNotification>(stringData);
            return new DepositNotification
            {
                Amount = command.Amount,
       
                Message = command.Message,
                IssuedBy = userInfoToken.Id,
               
                ApprovedBy = "NOT SET",
                IsApproved = false,
                Status = "Pending",
                ApprovedDate = new DateTime(),

            };
        }

    }


}
