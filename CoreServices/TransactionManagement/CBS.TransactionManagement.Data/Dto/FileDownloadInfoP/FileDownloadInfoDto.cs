using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.FileDownloadInfoP
{
    public class FileDownloadInfoDto
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string DownloadPath { get; set; }
        public string FileType { get; set; }
        public string FullPath { get; set; }
        public string Size { get; set; }
        public string UserId { get; set; }
        public string TransactionType { get; set; }
        public string UserName { get; set; }
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public DateTime DateInitiated { get; set; }

    }
    public class FileDownloadDto
    {
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
