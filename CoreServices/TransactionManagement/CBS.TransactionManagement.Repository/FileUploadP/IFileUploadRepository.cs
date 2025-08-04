using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.FileUploadP
{
    public interface IFileUploadRepository : IGenericRepository<FileUpload>
    {
        Task SaveFileUploadDetails(IFormFile file, string filePath, string fileHash, string branchId, string branchName, string fileUploadId, FileCategory fileCategory, SalaryProcessingStatus processingStatus, string fileCode="N/A");
    }
}
