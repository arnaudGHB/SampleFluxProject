using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddFileDownloadInfoCommand : IRequest<ServiceResponse<FileDownloadInfoDto>>
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string DownloadPath { get; set; }
        public string FileType { get; set; }
        public string FullPath { get; set; }
        public string Size { get; set; }
        public string TransactionType { get; set; }
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
    }

}
