using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Teller by its unique identifier.
    /// </summary>
    public class GetCurrentProvisionPrimaryTellerQuery : IRequest<ServiceResponse<List<CurrentProvisionPrimaryTellerDto>>>
    {
      
    }
}
