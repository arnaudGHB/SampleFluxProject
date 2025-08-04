using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity.Customers
{
    public class CustomerAgeCategory : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}
