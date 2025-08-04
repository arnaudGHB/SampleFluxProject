using CBS.CUSTOMER.DATA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.PinValidation
{
    public class ChangePinResponse
    {
        public string? Telephone { get; set; }
        public int? NumberOfFailedTries { get; set; }
        public bool ValidationStatus { get; set; }
        public string? LoginStatus { get; set; }
        public string? Token { get; set; }
        public CustomerDto? Customer { get; set; }
    }
}
