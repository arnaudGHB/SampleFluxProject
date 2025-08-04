using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Customers
{
    public class CreateQuestion
    {
        public string? SecretQuestion { get; set; }
        public string? SecretAnswer { get; set; }
        public string? CustomerId { get; set; }
    }
}
