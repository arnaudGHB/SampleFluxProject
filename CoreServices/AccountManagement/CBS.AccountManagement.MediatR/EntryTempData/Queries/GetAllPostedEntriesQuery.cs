using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllPostedEntriesQuery : IRequest<ServiceResponse<List<PostedEntryDto>>>
    {
        //public string ReferenceId { get; set; }
    }
}