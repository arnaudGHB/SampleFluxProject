using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.MediatR.FileDownloadInfoP.Queries
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
