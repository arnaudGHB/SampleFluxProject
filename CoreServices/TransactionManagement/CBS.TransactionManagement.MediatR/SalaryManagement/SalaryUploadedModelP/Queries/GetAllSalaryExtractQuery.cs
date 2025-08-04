using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries
{
    public class GetAllSalaryExtractQuery : IRequest<ServiceResponse<List<SalaryExtractDto>>>
    {
        public string FileUploadId { get; set; }
        public string? BranchId { get; set; }  // Optional filter
    }

}
