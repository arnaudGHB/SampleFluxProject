using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class JobTitle : BaseEntity
    {
        [Key]
        public int JobTitleId { get; set; }
        public string? Title { get; set; }
        public decimal SalaryMidpoint { get; set; }

    }
}
