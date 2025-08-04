using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.FileDownloadInfoP.Queries
{
    public class DownloadFileByIdQuery : IRequest<ServiceResponse<FileDownloadDto>>
    {
        public string FileId { get; set; }

        public DownloadFileByIdQuery(string fileId)
        {
            FileId = fileId;
        }
    }
}
