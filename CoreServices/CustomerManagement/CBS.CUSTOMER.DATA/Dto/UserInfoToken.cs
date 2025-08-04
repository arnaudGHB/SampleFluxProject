using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{ 
    public class UserInfoToken
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string ConnectionId { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }
        public string BranchID { get; set; }
        public string BankID { get; set; }
        public string BranchCode { get; set; }
        public string BankCode { get; set; }
        public string BranchName { get; set; }
        public bool IsHeadOffice { get; set; }

    }

}
