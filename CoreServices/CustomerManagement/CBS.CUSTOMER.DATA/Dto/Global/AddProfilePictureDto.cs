using CBS.CUSTOMER.HELPER.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Global
{
    public class AddProfilePictureDto
    {
        public string? CustomerId { get; set; }
        public string? Name { get; set; }
        public string? ProfileType { get; set; }
        public string? picturePath { get; set; }
        public DateTime ModifiedDate { get; set; }
      
      
    }
}
