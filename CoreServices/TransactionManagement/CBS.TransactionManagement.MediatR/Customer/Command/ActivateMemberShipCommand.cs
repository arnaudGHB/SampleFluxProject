using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class ActivateMembershipCommand : IRequest<ServiceResponse<List<MemberActivationResponse>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be retrieved.
        /// </summary>
        public string CustomerId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public decimal Amount { get; set; }
        public string BranchCode { get; set; }
        public MemberFeePolicyDto MemberFeePolicy { get; set; }
        public string MembershipApprovalStatus { get; set; }
        public ActivateMembershipCommand(string CustomerId, decimal Amount, string branchId, string bankId, MemberFeePolicyDto MemberFeePolicy,string bracnhCode)
        {
            this.CustomerId = CustomerId;
            this.Amount = Amount;
            this.MemberFeePolicy= MemberFeePolicy;
            BranchId = branchId;
            BankId = bankId;
            BranchCode=bracnhCode;
        }
    }
    public class ActivateMember
    {
        public string MembershipApprovalStatus { get; set; }
        public string CustomerId { get; set; }
        public string BranchCode { get; set; }
        public bool IsAutomatic { get; set; }

    }
    public class MemberActivationResponse
    {
        public string ServiceName { get; set; }
        public string EventCode { get; set; }
        public decimal Fee { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
    }
}
