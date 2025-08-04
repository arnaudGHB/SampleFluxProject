using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
  

    public class TransactionReversalRequest : BaseEntity
    {
        public string Id { get; set; }
        public string ReferenceId { get; set; } // opening balance reference id
     
 
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public string RequestMessage { get; set; }
        public string ApprovedMessage { get; set; }
        public string Status { get; set; }
        public static TransactionReversalRequest SetTransactionReversalRequest(string ReferenceNumber, string RequestMessage, UserInfoToken userInfoToken)
        {
                     return new TransactionReversalRequest
            {
                ReferenceId =  ReferenceNumber,
                
                RequestMessage = RequestMessage,
           
                IssuedBy = userInfoToken.Id,
                ApprovedBy = "NOT SET",
                ApprovedMessage = "",
                IsApproved = false,
                Status = "Pending",

                ApprovedDate = new DateTime(),

            };
        }
    }
 
}
