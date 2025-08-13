using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Queries
{
    /// <summary>
    /// Query to retrieve the history of cash change operations based on filters.
    /// </summary>
    public class GetCashChangeHistoryQuery : IRequest<ServiceResponse<List<CashChangeHistoryDto>>>
    {
        public string VaultId { get; set; }         // Filter by Vault ID
        public string SubTellerId { get; set; }    // Filter by Sub Teller ID
        public string PrimaryTellerId { get; set; } // Filter by Primary Teller ID
        public string BranchId { get; set; }       // Filter by Branch ID
        public DateTime? StartDate { get; set; }   // Filter by start date
        public DateTime? EndDate { get; set; }     // Filter by end date
        public string UserId { get; set; }         // Filter by User ID
    }


}
