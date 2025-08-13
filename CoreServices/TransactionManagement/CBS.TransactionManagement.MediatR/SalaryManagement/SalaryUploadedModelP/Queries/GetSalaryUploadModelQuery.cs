using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Queries
{
    /// <summary>
    /// Represents a query to get SalaryUploadModels by date, file upload ID, or salary code.
    /// </summary>
    public class GetSalaryUploadModelQuery : IRequest<ServiceResponse<List<SalaryUploadModelDto>>>
    {
        public DateTime? Date { get; set; } // Optional filter by date.
        public string? FileUploadId { get; set; } // Optional filter by file upload ID.
        public string? SalaryCode { get; set; } // Optional filter by salary code.
    }
}
