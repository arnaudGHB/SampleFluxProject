using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountCategory based on its unique identifier.
    /// </summary>
    public class GetReportQueryHandler : IRequestHandler<GetReportQuery, ServiceResponse<ReportDto>>
    {
        private readonly IReportDownloadRepository _reportDownloadRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetReportQueryHandler> _logger; // Logger for logging handler actions and errors.

 
        public GetReportQueryHandler(
            IReportDownloadRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<GetReportQueryHandler> logger)
        {
            _reportDownloadRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

   
        public async Task<ServiceResponse<ReportDto>> Handle(GetReportQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountCategory entity with the specified ID from the repository
                var entity = await _reportDownloadRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted) 
                    {
                        string message = "Report has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<ReportDto>.Return404(message);
                    }
                    else
                    {
                        // Map the AccountCategory entity to AccountCategoryDto and return it with a success response
                        var AccountCategoryDto = _mapper.Map<ReportDto>(entity);
                        return ServiceResponse<ReportDto>.ReturnResultWith200(AccountCategoryDto);
                    }
               
                }
                else
                {
                    // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Report not found.");
                    return ServiceResponse<ReportDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountCategory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<ReportDto>.Return500(e);
            }
        }
    }
}