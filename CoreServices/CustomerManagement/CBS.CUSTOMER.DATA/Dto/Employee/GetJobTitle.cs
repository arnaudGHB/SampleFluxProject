using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class GetJobTitle
    {
        public string? JobTitleId { get; set; }
        public string? Title { get; set; }
        public decimal SalaryMidpoint { get; set; }

    }
}
