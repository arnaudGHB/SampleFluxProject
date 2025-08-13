using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Queries
{
    /// <summary>
    /// Represents a query to retrieve Member None Cash Operations based on status and branch.
    /// </summary>
    public class GetAllMemberNoneCashOperationsQuery : IRequest<ServiceResponse<List<MemberNoneCashOperationDto>>>
    {
        /// <summary>
        /// The status of the operations to filter by (Pending, Approved, or Reject).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The BranchId to filter by (optional).
        /// </summary>
        public string BranchId { get; set; }

        public GetAllMemberNoneCashOperationsQuery(string status, string branchId = null)
        {
            Status = status;
            BranchId = branchId;
        }
    }

}
