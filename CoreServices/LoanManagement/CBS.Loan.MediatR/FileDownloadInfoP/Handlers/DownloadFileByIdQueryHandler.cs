using AutoMapper;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FileDownloadInfoP.Queries;
using CBS.NLoan.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FileDownloadInfoP.Handlers
{
    public class DownloadFileByIdQueryHandler : IRequestHandler<DownloadFileByIdQuery, ServiceResponse<FileDownloadDto>>
    {
        private readonly IConfiguration _configuration;
        private readonly IFileDownloadInfoRepository _fileDownloadInfoRepository; // Repository for accessing FileDownloadInfos data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DownloadFileByIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        public DownloadFileByIdQueryHandler(
            IFileDownloadInfoRepository fileDownloadInfoRepository,
            IMapper mapper,
            ILogger<DownloadFileByIdQueryHandler> logger,
            IConfiguration configuration)
        {
            _fileDownloadInfoRepository = fileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<FileDownloadDto>> Handle(DownloadFileByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch file download info entity based on fileId and check if it is not deleted.
                var entity = await _fileDownloadInfoRepository.FindAsync(request.FileId);

                if (entity == null)
                {
                    return ServiceResponse<FileDownloadDto>.Return404("File not found.");
                }

                var filePath = entity.FullPath;

                // Check if the file exists in the directory.
                if (!System.IO.File.Exists(filePath))
                {
                    return ServiceResponse<FileDownloadDto>.Return404("File not found on server.");
                }

                // Read file content as byte array.
                var fileData = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
                var contentType = GetContentType(filePath);

                // Create the FileDownloadDto.
                var fileDownloadDto = new FileDownloadDto
                {
                    FileData = fileData,
                    FileName = $"{entity.FileName}{entity.Extension}",
                    ContentType = contentType
                };

                return ServiceResponse<FileDownloadDto>.ReturnResultWith200(fileDownloadDto);
            }
            catch (Exception e)
            {
                // Log the exception and return a 500 Internal Server Error response with the error message.
                _logger.LogError($"Failed to download file: {e.Message}", e);
                return ServiceResponse<FileDownloadDto>.Return500(e, "Failed to download file.");
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
