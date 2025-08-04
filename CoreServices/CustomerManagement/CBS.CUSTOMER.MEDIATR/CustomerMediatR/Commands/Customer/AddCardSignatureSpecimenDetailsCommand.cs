
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.AspNetCore.Http;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new Membership Customer.
    /// </summary>
    public class AddCardSignatureSpecimenDetailsCommand : IRequest<ServiceResponse<CreateCardSignatureSpecimenDetails>>
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
