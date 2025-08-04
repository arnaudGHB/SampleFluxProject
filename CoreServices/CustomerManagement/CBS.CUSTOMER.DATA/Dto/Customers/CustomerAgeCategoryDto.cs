using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Customers
{
    public class CustomerAgeCategoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int from { get; set; }
        public int to { get; set; }
    }
}
