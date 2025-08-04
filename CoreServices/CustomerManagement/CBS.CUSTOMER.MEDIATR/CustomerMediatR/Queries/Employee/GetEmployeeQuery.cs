using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Employee by its unique identifier.
    /// </summary>
    public class GetEmployeeQuery : IRequest<ServiceResponse<GetEmployee>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Employee to be retrieved.
        /// </summary>
        public string EmployeeId { get; set; }
    }
}
