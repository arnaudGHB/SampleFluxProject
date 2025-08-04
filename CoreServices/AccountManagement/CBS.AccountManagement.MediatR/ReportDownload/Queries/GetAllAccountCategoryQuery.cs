using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllReportQuery : IRequest<ServiceResponse<List<ReportDto>>>
    {
    }
    public class DownloadFileByUserIdQuery : IRequest<ServiceResponse<List<ReportDto>>>
    {
        public string userId { get; set; }
    }
}