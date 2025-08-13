using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR
{
    public class GetAllCustomerListingsQuery : IRequest<ServiceResponse<List<CustomerDto>>>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? QueryParameter { get; set; }
        public string? BranchId { get; set; }
        public int PageSize { get; set; } = 100;
        public int PageNumber { get; set; } = 1;
        public string? MembersStatusType { get; set; } // None Members, Members, Both
        public string? LegalFormStatus { get; set; } // Moral_Person, Physical_Person, Both
    }

}
