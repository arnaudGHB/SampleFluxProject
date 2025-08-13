using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.FileDownloadInfoP.Queries
{
    public class GetAllFileDownloadInfoQuery : IRequest<ServiceResponse<List<FileDownloadInfoDto>>>
    {
    }
}
