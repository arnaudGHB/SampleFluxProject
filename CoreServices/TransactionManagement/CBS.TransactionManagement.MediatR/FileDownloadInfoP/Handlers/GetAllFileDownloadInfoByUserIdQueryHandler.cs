using AutoMapper;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Dto;
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
    public class GetAllFileDownloadInfoByUserIdQueryHandler : IRequestHandler<GetAllFileDownloadInfoByUserIdQuery, ServiceResponse<List<FileDownloadInfoDto>>>
    {
        private readonly IConfiguration _configuration;

        private readonly IFileDownloadInfoRepository _FileDownloadInfoRepository; // Repository for accessing FileDownloadInfos data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFileDownloadInfoByUserIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllFileDownloadInfoByUserIdQueryHandler.
        /// </summary>
        /// <param name="FileDownloadInfoRepository">Repository for FileDownloadInfos data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFileDownloadInfoByUserIdQueryHandler(
            IFileDownloadInfoRepository FileDownloadInfoRepository,
            IMapper mapper, ILogger<GetAllFileDownloadInfoByUserIdQueryHandler> logger, UserInfoToken userInfoToken = null, IConfiguration configuration = null)
        {
            // Assign provided dependencies to local variables.
            _FileDownloadInfoRepository = FileDownloadInfoRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
        }

        /// <summary>
        /// Handles the GetAllFileDownloadInfoByUserIdQuery to retrieve all FileDownloadInfos.
        /// </summary>
        /// <param name="request">The GetAllFileDownloadInfoByUserIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FileDownloadInfoDto>>> Handle(GetAllFileDownloadInfoByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _FileDownloadInfoRepository.FindBy(x => x.UserId == request.UserId && x.IsDeleted == false).ToListAsync();
                var dto = _mapper.Map<List<FileDownloadInfoDto>>(entities);
                return ServiceResponse<List<FileDownloadInfoDto>>.ReturnResultWith200(dto);
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
