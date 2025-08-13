using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FileDownloadInfoP.Commands
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
    }

}
