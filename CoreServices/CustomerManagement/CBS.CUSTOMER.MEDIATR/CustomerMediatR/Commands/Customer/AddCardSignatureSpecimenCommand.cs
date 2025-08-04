
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Customer;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new Card Signature Specimen.
    /// </summary>
    public class AddCardSignatureSpecimenCommand : IRequest<ServiceResponse<CreateCardSignatureSpecimen>>
    {

        public string? CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? BranchId { get; set; }
        public string? BranchMangerId { get; set; }
        public string? IHereByTestifyforAllTheSignatures { get; set; }
        public List<AddCardSignatureDetailsCommand>? CardSignatureDetails { get; set; }



    }

}
