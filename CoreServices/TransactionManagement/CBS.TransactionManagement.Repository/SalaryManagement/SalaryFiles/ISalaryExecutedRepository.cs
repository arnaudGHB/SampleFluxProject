using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using Microsoft.AspNetCore.Http;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles
{
    public interface ISalaryExecutedRepository : IGenericRepository<SalaryExtract>
    {

        Task<bool> ExtractExcelDataToDatabase(IFormFile file, string branchId, string branchName, string branchCode, string fileUploadId);
        Task RemoveSalaryExtractDataForFile(string fileUploadId);

    }
}
