using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class AddReportCommand : IRequest<ServiceResponse<ReportDto>>
    {

        public string Id { get; set; }
        public string ReportType { get; set; }
        public string Extension { get; set; }
        public string DownloadPath { get; set; }
        public string Size { get; set; }
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string BranchName
        {
            get; set;
        }
        public string Username { get; set; }
        public static AddReportCommand ConvertToReportCommand(ReportDto reportDto)
        {
            return new AddReportCommand
            {
                 BranchName = reportDto.BranchName,
                  Username = reportDto.Username,
                  FileName = reportDto.FileName,
                  FileType = reportDto.FileType,
                   FullPath = reportDto.FullPath,
                   Size = reportDto.Size,
                   DownloadPath = reportDto.DownloadPath,
                   Extension = reportDto.Extension,
                   ReportType = reportDto.ReportType,
                   Id = reportDto.Id
            };
        }
    }
}