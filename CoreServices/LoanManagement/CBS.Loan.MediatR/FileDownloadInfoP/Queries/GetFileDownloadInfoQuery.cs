using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FileDownloadInfoP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetFileDownloadInfoQuery : IRequest<ServiceResponse<FileDownloadInfoDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the FileDownloadInfo to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
