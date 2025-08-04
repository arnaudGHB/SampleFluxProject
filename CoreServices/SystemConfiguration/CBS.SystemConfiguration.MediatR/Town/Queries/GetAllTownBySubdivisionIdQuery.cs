using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Queries
{
    public class GetAllTownBySubdivisionIdQuery : IRequest<ServiceResponse<List<TownDto>>>
    {
        public string Id { get; set; }
    }
}