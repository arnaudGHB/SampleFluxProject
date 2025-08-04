using AutoMapper;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.FileUploadP.Queries;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileDownloadInfoP;
using CBS.TransactionManagement.Repository.FileUploadP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileUploadP.Handlers
{
    public class DownloadFileByIdQueryHandler : IRequestHandler<DownloadFileByIdQuery, ServiceResponse<FileDownloadDto>>
    {
        private readonly IFileUploadRepository _fileUploadRepository; // Repository for accessing FileDownloadInfos data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DownloadFileByIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        public DownloadFileByIdQueryHandler(
            IFileUploadRepository fileDownloadInfoRepository,
            IMapper mapper,
            ILogger<DownloadFileByIdQueryHandler> logger)
        {
            _fileUploadRepository = fileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<FileDownloadDto>> Handle(DownloadFileByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch file download info entity based on fileId and check if it is not deleted.
                var entity = await _fileUploadRepository.FindAsync(request.FileId);

                if (entity == null)
                {
                    return ServiceResponse<FileDownloadDto>.Return404("AttachedFiles not found.");
                }

                var filePath = entity.FilePath;

                // Check if the file exists in the directory.
                if (!System.IO.File.Exists(filePath))
                {
                    return ServiceResponse<FileDownloadDto>.Return404("AttachedFiles not found on server.");
                }

                // Read file content as byte array.
                var fileData = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
                var contentType = BaseUtilities.GetContentType(filePath);

                // Create the FileDownloadDto.
                var fileDownloadDto = new FileDownloadDto
                {
                    FileData = fileData,
                    FileName = $"{entity.FileName}",
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

       
    }
}
