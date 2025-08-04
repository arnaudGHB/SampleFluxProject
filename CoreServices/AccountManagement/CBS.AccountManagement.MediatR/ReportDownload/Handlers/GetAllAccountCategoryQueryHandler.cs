using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class GetAllReportQueryHandler : IRequestHandler<GetAllReportQuery, ServiceResponse<List<ReportDto>>>
    {
        private readonly IReportDownloadRepository _reportDownloadRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllReportQueryHandler> _logger; // Logger for logging handler actions and errors.


        public GetAllReportQueryHandler(
            IReportDownloadRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<GetAllReportQueryHandler> logger)
        {
            _reportDownloadRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }
 

        /// <summary>
        /// Handles the GetAllAccountCategoryQuery to retrieve all AccountCategorys.
        /// </summary>
        /// <param name="request">The GetAllAccountCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ReportDto>>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountCategorys entities from the repository
                var entities = await _reportDownloadRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<ReportDto>>.ReturnResultWith200(_mapper.Map<List<ReportDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ReportDtos: {e.Message}");
                return ServiceResponse<List<ReportDto>>.Return500(e, "Failed to get all ReportDto");
            }
        }
    }

    public class DownloadFileByUserIdQueryHandler : IRequestHandler<DownloadFileByUserIdQuery, ServiceResponse<List<ReportDto>>>
    {
        private readonly IReportDownloadRepository _reportDownloadRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DownloadFileByUserIdQueryHandler> _logger; // Logger for logging handler actions and errors.


        public DownloadFileByUserIdQueryHandler(
            IReportDownloadRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<DownloadFileByUserIdQueryHandler> logger)
        {
            _reportDownloadRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }


        /// <summary>
        /// Handles the GetAllAccountCategoryQuery to retrieve all AccountCategorys.
        /// </summary>
        /// <param name="request">The GetAllAccountCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ReportDto>>> Handle(DownloadFileByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountCategorys entities from the repository
                var entities = await _reportDownloadRepository.All.Where(x => x.IsDeleted.Equals(false)&&x.CreatedBy.Equals(request.userId)).ToListAsync();
                return ServiceResponse<List<ReportDto>>.ReturnResultWith200(_mapper.Map<List<ReportDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ReportDtos: {e.Message}");
                return ServiceResponse<List<ReportDto>>.Return500(e, "Failed to get all ReportDto");
            }
        }
    }
}