using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class CustomerCategory:BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
