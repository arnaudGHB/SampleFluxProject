using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class Department : BaseEntity
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

    }
}
