using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Queries
{
    public class GetAllCountryQuery : IRequest<ServiceResponse<List<CountryDto>>>
    {

    }
}