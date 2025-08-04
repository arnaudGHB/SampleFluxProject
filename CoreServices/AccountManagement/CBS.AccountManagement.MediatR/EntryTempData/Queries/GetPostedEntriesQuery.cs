using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetPostedEntriesQuery : IRequest<ServiceResponse<PostedEntryDto>>
    {
        public string ReferenceId { get; set; }
    }
}