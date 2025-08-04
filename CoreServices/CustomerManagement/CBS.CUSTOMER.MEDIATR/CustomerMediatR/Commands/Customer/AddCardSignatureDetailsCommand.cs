using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Customer
{
    public  class AddCardSignatureDetailsCommand
    {
        public string? CardSignatureSpecimenId { get; set; }
        public string? Name { get; set; }
        public string? IdentityCardNumber { get; set; }
        public string? IssuedAt { get; set; }
        public string? IssuedOn { get; set; }
        public string? SignatureUrl1 { get; set; }
        public string? SignatureUrl2 { get; set; }
        public string? PhotoUrl1 { get; set; }
        public string? Instruction { get; set; }
    }
}
