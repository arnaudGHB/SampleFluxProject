using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Queries
{
    public class GetAllDivisionByRegionIdQuery : IRequest<ServiceResponse<List<DivisionDto>>>
    {
        public string Id { get; set; }
    }
}