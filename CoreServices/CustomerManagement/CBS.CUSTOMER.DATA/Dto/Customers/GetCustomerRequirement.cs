using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class GetCustomerRequirement:BaseEntity
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Discription { get; set; }

        public string? CustomerId { get; set; }
        public string? UserId { get; set; }

    }
}
