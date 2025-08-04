using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.Queries
{
    public class GetMembersWithMatriculeByBranchQuery : IRequest<ServiceResponse<List<CustomerDto>>>
    {
        public bool IsMatricule { get; set; }
        public string BranchId { get; set; }
    }
}
