using AutoMapper;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.FileDownloadInfoP.Queries;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileDownloadInfoP.Handlers
{
    public class DownloadFileByIdQueryHandler : IRequestHandler<DownloadFileByIdQuery, ServiceResponse<FileDownloadDto>>
    {
        private readonly IFileDownloadInfoRepository _fileDownloadInfoRepository; // Repository for accessing FileDownloadInfos data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DownloadFileByIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        public DownloadFileByIdQueryHandler(
            IFileDownloadInfoRepository fileDownloadInfoRepository,
            IMapper mapper,
            ILogger<DownloadFileByIdQueryHandler> logger)
        {
            _fileDownloadInfoRepository = fileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<FileDownloadDto>> Handle(DownloadFileByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch file download info entity based on fileId and check if it is not deleted.
                var entity = await _fileDownloadInfoRepository.FindAsync(request.FileId);

                if (entity == null)
                {
                    return ServiceResponse<FileDownloadDto>.Return404("AttachedFiles not found.");
                }

                var filePath = entity.FullPath;

                // Check if the file exists in the directory.
                if (!System.IO.File.Exists(filePath))
                {
                    return ServiceResponse<FileDownloadDto>.Return404("AttachedFiles not found on server.");
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
