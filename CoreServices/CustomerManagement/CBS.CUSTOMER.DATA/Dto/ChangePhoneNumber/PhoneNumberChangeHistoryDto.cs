using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.ChangePhoneNumber
{
    /// <summary>
    /// Represents the history of phone number changes for a customer.
    /// </summary>
    public class PhoneNumberChangeHistoryDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string OldPhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }
        public string ChangedByName { get; set; }
        public string Reason { get; set; }
        public string MemberName { get; set; }
        public DateTime ChangeDate { get; set; }
    }

}
