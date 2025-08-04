using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllEntryTempDataQuery : IRequest<ServiceResponse<List<EntryTempDataDto>>>
    {
        public string ReferenceId { get; set; }
    }
}