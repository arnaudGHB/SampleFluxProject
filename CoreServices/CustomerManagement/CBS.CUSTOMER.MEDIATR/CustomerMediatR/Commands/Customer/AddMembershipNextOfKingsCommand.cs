
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.AspNetCore.Http;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new Membership Next Of Kings.
    /// </summary>
    public class AddMembershipNextOfKingsCommand : IRequest<ServiceResponse<CreateMembershipNextOfKings>>
    {

       public string? Name { get; set; }
        public string? CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? Relation { get; set; }
        public string? Ratio { get; set; }
        public string? SignatureUrl { get; set; }
        public string? PhotoUrl { get; set; }
        public string? BranchId { get; set; }




    }

}
