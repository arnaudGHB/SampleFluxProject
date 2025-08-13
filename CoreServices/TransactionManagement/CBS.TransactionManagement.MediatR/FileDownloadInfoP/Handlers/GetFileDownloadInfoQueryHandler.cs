using AutoMapper;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.FileDownloadInfoP.Queries;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileDownloadInfoP;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetFileDownloadInfoQueryHandler : IRequestHandler<GetFileDownloadInfoQuery, ServiceResponse<FileDownloadInfoDto>>
    {
        private readonly IFileDownloadInfoRepository _FileDownloadInfoRepository; // Repository for accessing FileDownloadInfo data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFileDownloadInfoQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for initializing the GetFileDownloadInfoQueryHandler.
        /// </summary>
        /// <param name="FileDownloadInfoRepository">Repository for FileDownloadInfo data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetFileDownloadInfoQueryHandler(
            IFileDownloadInfoRepository FileDownloadInfoRepository,
            IMapper mapper,
            ILogger<GetFileDownloadInfoQueryHandler> logger,
            IConfiguration configuration = null)
        {
            _FileDownloadInfoRepository = FileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Handles the GetFileDownloadInfoQuery to retrieve a specific FileDownloadInfo.
        /// </summary>
        /// <param name="request">The GetFileDownloadInfoQuery containing FileDownloadInfo ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FileDownloadInfoDto>> Handle(GetFileDownloadInfoQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the FileDownloadInfo entity with the specified ID from the repository
                var entity = await _FileDownloadInfoRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the FileDownloadInfo entity to FileDownloadInfoDto and return it with a success response
                    var FileDownloadInfoDto = _mapper.Map<FileDownloadInfoDto>(entity);


                    // Get the dynamically obtained domain name
                    var serverDomain = _configuration["ServerSettings:DomainName"];
                    // Map entities to DTOs and set the DownloadPath
                    FileDownloadInfoDto.DownloadPath = $"{serverDomain}{entity.DownloadPath}";


                    return ServiceResponse<FileDownloadInfoDto>.ReturnResultWith200(FileDownloadInfoDto);
                }
                else
                {
                    // If the FileDownloadInfo entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("FileDownloadInfo not found.");
                    return ServiceResponse<FileDownloadInfoDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting FileDownloadInfo: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<FileDownloadInfoDto>.Return500(e);
            }
        }
    }

}
