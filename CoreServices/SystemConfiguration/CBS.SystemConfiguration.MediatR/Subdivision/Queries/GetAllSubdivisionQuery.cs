using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Queries
{
    public class GetAllSubdivisionQuery : IRequest<ServiceResponse<List<SubdivisionDto>>>
    {

    }
}