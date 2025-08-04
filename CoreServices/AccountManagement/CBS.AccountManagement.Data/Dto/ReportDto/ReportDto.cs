using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class ReportDto
    {
        public string Id { get; set; }
        public string ReportType { get; set; }
        public string Extension { get; set; }
        public string DownloadPath { get; set; }
        public string Size { get; set; }
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string BranchName { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class FileReportInfoDto
    {
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
