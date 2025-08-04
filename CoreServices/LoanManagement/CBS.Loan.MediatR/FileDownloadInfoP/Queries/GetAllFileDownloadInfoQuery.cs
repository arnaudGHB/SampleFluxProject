using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FileDownloadInfoP.Queries
{
    public class GetAllFileDownloadInfoQuery : IRequest<ServiceResponse<List<FileDownloadInfoDto>>>
    {
    }
}
