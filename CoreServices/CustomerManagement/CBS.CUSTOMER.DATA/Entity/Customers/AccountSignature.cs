using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class AccountSignature:BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string? IHereByTestifyforAllTheSignatures { get; set; }
        public string Name { get; set; }
        public string IdentityCardNumber { get; set; }
        public string IssuedAt { get; set; }
        public string IssuedOn { get; set; }
        public string? SignatureUrl1 { get; set; }
        public string? SignatureUrl2 { get; set; }
        public string? PhotoUrl1 { get; set; }
        public string? Instruction { get; set; }
        public string? SpecialInstruction { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
