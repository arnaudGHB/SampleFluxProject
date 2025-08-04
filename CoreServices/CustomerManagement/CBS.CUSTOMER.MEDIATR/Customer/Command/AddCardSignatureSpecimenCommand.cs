
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new Card Signature Specimen.
    /// </summary>
    public class AddCardSignatureSpecimenCommand : IRequest<ServiceResponse<CreateCardSignatureSpecimen>>
    {

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



    }

}
