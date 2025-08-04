using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.FileDownloadInfoP.Queries
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
