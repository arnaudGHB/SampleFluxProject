using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    /// <summary>
    /// Query to retrieve paginated, filtered, and sortable loan data for DataTable.
    /// </summary>
    public class GetLoansDataTableQuery : IRequest<ServiceResponse<CustomDataTable>>
    {
        /// <summary>
        /// DataTable options containing pagination, sorting, and search parameters.
        /// </summary>
        public DataTableOptions DataTableOptions { get; set; }

        /// <summary>
        /// Optional filter to retrieve loans from a specific branch.
        /// </summary>
        public string BranchId { get; set; }

        /// <summary>
        /// Optional filter to retrieve loans for a specific customer or member.
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// Optional loan status filter. 
        /// Possible values: "Open", "Closed", "Refinanced", "Restructured", "Rescheduled".
        /// Use "all" to include all statuses.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Optional start date to filter loans based on the loan creation date.
        /// Only loans created on or after this date will be included.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Optional end date to filter loans based on the loan creation date.
        /// Only loans created on or before this date will be included.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Optional filter to retrieve loans based on delinquency status.
        /// Possible values: "Current", "Delinquent", or "all".
        /// </summary>
        public string? DeliquentStatus { get; set; }
    }


}
