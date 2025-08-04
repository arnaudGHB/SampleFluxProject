using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.MediatR.FileDownloadInfoP.Queries
{
    public class GetAllFileDownloadInfoByUserIdQuery : IRequest<ServiceResponse<List<FileDownloadInfoDto>>>
    {
        public string UserId { get; set; }
    }
   
}
