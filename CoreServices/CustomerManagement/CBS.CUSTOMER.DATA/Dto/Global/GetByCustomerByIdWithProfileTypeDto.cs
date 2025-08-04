// Ignore Spelling: Dto

using CBS.CUSTOMER.HELPER.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Global
{
    public class GetByCustomerByIdWithProfileTypeDto
    {
        public string? CustomerId { get; set; }
        public string? Name { get; set; }
        public string? ProfileType { get; set; }
        public DateTime CreationDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Phone { get; set; }
        public string? IDNumber { get; set; }

        public string? Email { get; set; }
      
    }
}
