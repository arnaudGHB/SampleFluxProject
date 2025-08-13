using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands
{
    public class SalaryAnalysisCommand : IRequest<ServiceResponse<SalaryAnalysisResultDto>>
    {
        public string FileUploadId { get; set; }
    }
}
