using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber
{
    /// <summary>
    /// Represents the history of phone number changes for a customer.
    /// </summary>
    public class PhoneNumberChangeHistory:BaseEntity
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string MemberName { get; set; }
        public string OldPhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }
        public string RequestBy { get; set; }
        public string ApprovedBy { get; set; }
        public string RequestUserId { get; set; }
        public string ApprovalUserId { get; set; }
        public DateTime DateOfRequest { get; set; }
        public DateTime ApproveDate { get; set; }
        public string RequestComment { get; set; }
        public string ApprovalComment { get; set; }
        public string Status { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
    }

}
