using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class GetBranchByIdCommand : IRequest<ServiceResponse<BranchDto>>
    {
        public string BranchId { get; set; }
 
    }

   
}
