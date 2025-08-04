using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class GetAllCustomerCategory
    {
        public string? CustomerCategoryId { get; set; }
        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }
    }
}
