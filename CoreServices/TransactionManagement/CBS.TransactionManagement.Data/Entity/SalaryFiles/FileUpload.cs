using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.SalaryFiles
{
    public class FileUpload:BaseEntity
    {
        public string Id { get; set; } // Unique Identifier for the file
        public string FileName { get; set; } // Original file name
        public string FilePath { get; set; } // Path where the file is stored
        public string FileHash { get; set; } // Unique hash to prevent duplicate uploads
        public string BranchId { get; set; } // Branch Id associated with the upload
        public string BranchName { get; set; } // Branch name associated with the upload
        public string UploadedBy { get; set; } // User who uploaded the file
        public DateTime UploadedOn { get; set; } // Timestamp of when the file was uploaded
        public string FileUploadId { get; set; } // Reference id for file tracking
        public string FileCategory { get; set; }
        public string SalaryProcessingStatus { get; set; }
        public bool IsAvalaibleForExecution { get; set; }
        public string? FileCode { get; set; }
        public int TotalBranchesThatHaveExecutedPayrol { get; set; }
        public int TotalBranchesInvolvedInPayrolProcessing { get; set; }
        public virtual ICollection<SalaryUploadModel> SalaryUploadModels { get; set; }

    }
}
