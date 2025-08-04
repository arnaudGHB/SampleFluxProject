using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.FileUploadP
{

    public class FileUploadRepository : GenericRepository<FileUpload, TransactionContext>, IFileUploadRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<SalaryProcessingRepository> _logger;

        public FileUploadRepository(IUnitOfWork<TransactionContext> unitOfWork, UserInfoToken userInfoToken, ILogger<SalaryProcessingRepository> logger) : base(unitOfWork)
        {
            _userInfoToken=userInfoToken;
            _logger=logger;
        }
        public async Task SaveFileUploadDetails(IFormFile file, string filePath, string fileHash, string branchId, string branchName, string fileUploadId, FileCategory fileCategory, SalaryProcessingStatus processingStatus,string fileCode)
        {
            var fileUpload = new FileUpload
            {
                Id = fileUploadId, FileCode=fileCode,
                FileName = file.FileName,
                FilePath = filePath,
                FileHash = fileHash,
                BranchId = branchId,
                BranchName = branchName,
                UploadedBy = _userInfoToken.FullName,
                UploadedOn = BaseUtilities.UtcNowToDoualaTime(),
                FileUploadId = fileUploadId,
                FileCategory =fileCategory.ToString(),
                SalaryProcessingStatus = SalaryProcessingStatus.Extracted.ToString()
            };

            Add(fileUpload);
            _logger.LogInformation("File details saved successfully.");
        }
    }
}
