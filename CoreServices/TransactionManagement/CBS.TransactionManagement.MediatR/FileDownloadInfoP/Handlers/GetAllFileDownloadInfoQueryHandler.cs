using AutoMapper;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.FileDownloadInfoP.Queries;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllFileDownloadInfoQueryHandler : IRequestHandler<GetAllFileDownloadInfoQuery, ServiceResponse<List<FileDownloadInfoDto>>>
    {
        private readonly IFileDownloadInfoRepository _fileDownloadInfoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllFileDownloadInfoQueryHandler> _logger;
        private readonly IConfiguration _configuration;

        public GetAllFileDownloadInfoQueryHandler(
            IFileDownloadInfoRepository fileDownloadInfoRepository,
            IMapper mapper,
            ILogger<GetAllFileDownloadInfoQueryHandler> logger,
            IConfiguration configuration)
        {
            _fileDownloadInfoRepository = fileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<List<FileDownloadInfoDto>>> Handle(GetAllFileDownloadInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all FileDownloadInfos entities from the repository
                var entities = await _fileDownloadInfoRepository.All.Where(x => !x.IsDeleted).ToListAsync();

                // Get the dynamically obtained domain name
                var serverDomain = _configuration["ServerSettings:DomainName"];

                // Map entities to DTOs and set the DownloadPath
                var dtos = entities.Select(entity =>
                {
                    var dto = _mapper.Map<FileDownloadInfoDto>(entity);
                    dto.DownloadPath = $"{serverDomain}{entity.DownloadPath}";
                    return dto;
                }).ToList();

                return ServiceResponse<List<FileDownloadInfoDto>>.ReturnResultWith200(dtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all FileDownloadInfos: {e.Message}");
                return ServiceResponse<List<FileDownloadInfoDto>>.Return500(e, "Failed to get all FileDownloadInfos");
            }
        }
    }
}
