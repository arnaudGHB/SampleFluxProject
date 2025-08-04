using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class InitiateDownloadInfoCommand : IRequest<ServiceResponse<FileDownloadInfoDto>>
    {
        public bool IsByBranch { get; set; }
        public string BranchId { get; set; }
        public bool IsUnpaidOnly { get; set; }
        public string? QueryParameter { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
        public string BranchName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
