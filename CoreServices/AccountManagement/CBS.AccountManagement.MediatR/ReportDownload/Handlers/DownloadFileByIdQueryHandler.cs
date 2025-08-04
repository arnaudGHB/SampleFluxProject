using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class DownloadFileByIdQueryHandler : IRequestHandler<DownloadFileByIdQuery, ServiceResponse<FileReportInfoDto>>
    {
        private readonly IReportDownloadRepository _fileDownloadInfoRepository; // Repository for accessing FileDownloadInfos data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DownloadFileByIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        public DownloadFileByIdQueryHandler(
            IReportDownloadRepository fileDownloadInfoRepository,
            IMapper mapper,
            ILogger<DownloadFileByIdQueryHandler> logger)
        {
            _fileDownloadInfoRepository = fileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<FileReportInfoDto>> Handle(DownloadFileByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch file download info entity based on fileId and check if it is not deleted.
                var entity = await _fileDownloadInfoRepository.FindAsync(request.FileId);

                if (entity == null)
                {
                    return ServiceResponse<FileReportInfoDto>.Return404("File not found.");
                }

                var filePath = entity.FullPath;

                // Check if the file exists in the directory.
                if (!System.IO.File.Exists(filePath))
                {
                    return ServiceResponse<FileReportInfoDto>.Return404("File not found on server.");
                }

                // Read file content as byte array.
                var fileData = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
                var contentType = GetContentType(filePath);

                // Create the FileDownloadDto.
                var fileDownloadDto = new FileReportInfoDto
                {
                    FileData = fileData,
                    FileName = $"{entity.FileName}{entity.Extension}",
                    ContentType = contentType
                };

                return ServiceResponse<FileReportInfoDto>.ReturnResultWith200(fileDownloadDto);
            }
            catch (Exception e)
            {
                // Log the exception and return a 500 Internal Server Error response with the error message.
                _logger.LogError($"Failed to download file: {e.Message}", e);
                return ServiceResponse<FileReportInfoDto>.Return500(e, "Failed to download file.");
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"}
        };
        }
    }
}
