using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FileUploadP.Queries
{
    public class GetAllFileUploadSalaryFileActivatedQuery : IRequest<ServiceResponse<List<FileUploadDto>>>
    {
        public bool Status { get; set; }
        public bool Both { get; set; }
    }
}
