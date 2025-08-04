

// Ignore Spelling: Dto

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class GetCardSignatureSpecimenDetails
    {
        public string? Id { get; set; }
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
