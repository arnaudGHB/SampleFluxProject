using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Queries
{
    public class GetLoanApplicationsDataTableQuery : IRequest<ServiceResponse<CustomDataTable>>
    {
        public DataTableOptions DataTableOptions { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? MemberId { get; set; }
        public string? BranchId { get; set; }
        public string? Status { get; set; } // Pending, Approved, Rejected, or All

        // New fields added
        public string? LoanCategory { get; set; } // Main, Special Saving Facilities, etc.
        public string? LoanTarget { get; set; } // Employee, Government, Groups, etc.
        public string? ApprovalStatus { get; set; } // Approved, Pending Approval, Rejected, All
    }

}
