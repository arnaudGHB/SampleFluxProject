using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class CashReplenishment : BaseEntity
    {
        public string Id { get; set; }
        public string ReferenceId { get; set; } // opening balance reference id
        public decimal AmountRequested { get; set; }
        public decimal AmountApproved { get; set; }
        public string RequestMessage { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public string CurrencyCode { get; set; } = "XAF";
        public string ApprovedMessage { get; set; }
        public string CashRequisitionType { get; set; } = "REQUEST";
        public string Status { get; set; }
        public string ParentCashReplenishId { get; set; } = "xxx";
        public string CorrespondingBranchId { get; set; }
        public string CashReplishmentRequestStatus { get; set; } = "Pendding";
    
        public static CashReplenishment SetCashInfusionCommand(string stringData, UserInfoToken userInfoToken)
        {
            AddCashInfusion command = JsonConvert.DeserializeObject<AddCashInfusion>(stringData);
            return new CashReplenishment
            {
                ReferenceId = command.ReferenceNumber,
                AmountRequested = command.Amount,
                RequestMessage = command.RequestMessage,
                CurrencyCode = command.CurrencyCode,
                IssuedBy = userInfoToken.Id,
                ApprovedBy = "NOT SET",
                ApprovedMessage = "",
                IsApproved = false,
                Status = "Pending",
                CorrespondingBranchId = "xxx",
                ApprovedDate = new DateTime(),

            };
        }
    }
    public class AddCashInfusion
    {
        public decimal Amount { get; set; }
        public string RequestMessage { get; set; }
        public string ReferenceNumber { get; set; }
        public string CurrencyCode { get; set; }
    }
}
