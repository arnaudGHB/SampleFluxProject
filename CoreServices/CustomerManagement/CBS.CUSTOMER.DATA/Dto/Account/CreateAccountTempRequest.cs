using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateAccountTempRequest
    {
        public string? ProductId { get; set; }
        public string? CustomerId { get; set; }
        public string? BankId { get; set; }
        public string? BranchId { get; set; }
    }
}
