using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Queries
{
    public class GetAllSubdivisionByDivisionIdQuery : IRequest<ServiceResponse<List<SubdivisionDto>>>
    {
        public string Id { get; set; }
    }
}