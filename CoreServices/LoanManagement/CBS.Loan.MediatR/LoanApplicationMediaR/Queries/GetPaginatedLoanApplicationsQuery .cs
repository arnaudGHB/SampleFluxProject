using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Queries
{
    public class GetPaginatedLoanApplicationsQuery : IRequest<ServiceResponse<PaginatedResult<LoanApplicationDto>>>
    {
        public string ParamMeter { get; set; } // Pending, Approved, Rejected, All
        public string BranchId { get; set; } // Branch to filter by
        public string MemberId { get; set; } // Member to filter by
        public DateTime? StartDate { get; set; } // Optional start date for query
        public DateTime? EndDate { get; set; } // Optional end date for query
        public int PageNumber { get; set; } = 1; // Default to page 1
        public int PageSize { get; set; } = 10;  // Default to 10 items per page

        public GetPaginatedLoanApplicationsQuery(string paramMeter, string branchId, string memberId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            ParamMeter = paramMeter;
            BranchId = branchId;
            MemberId = memberId;
            StartDate = startDate;
            EndDate = endDate;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

}
