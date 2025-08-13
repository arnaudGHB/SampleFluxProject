using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.FileDownloadInfoP.Queries
{
    public class GetAllFileDownloadInfoByUserIdQuery : IRequest<ServiceResponse<List<FileDownloadInfoDto>>>
    {
        public string UserId { get; set; }
    }
}
