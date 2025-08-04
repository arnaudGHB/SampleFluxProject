using AutoMapper;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FileUploadP.Queries;
using CBS.TransactionManagement.Repository.FileUploadP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileUploadMediaR.FileUploadP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllFileUploadSalaryFileActivatedQueryHandler : IRequestHandler<GetAllFileUploadSalaryFileActivatedQuery, ServiceResponse<List<FileUploadDto>>>
    {
        private readonly IFileUploadRepository _FileUploadRepository; // Repository for accessing FileUploads data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFileUploadSalaryFileActivatedQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllFileUploadQueryHandler.
        /// </summary>
        /// <param name="FileUploadRepository">Repository for FileUploads data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFileUploadSalaryFileActivatedQueryHandler(
            IFileUploadRepository FileUploadRepository,
            IMapper mapper, ILogger<GetAllFileUploadSalaryFileActivatedQueryHandler> logger, UserInfoToken userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _FileUploadRepository = FileUploadRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken=userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllFileUploadSalaryFileActivatedQuery to retrieve all FileUploads based on filters.
        /// </summary>
        /// <param name="request">The GetAllFileUploadSalaryFileActivatedQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FileUploadDto>>> Handle(GetAllFileUploadSalaryFileActivatedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Build the query dynamically based on the command parameters
                var query = _FileUploadRepository.FindBy(x => !x.IsDeleted && x.FileCategory == "SalaryModelExtraction");

                if (!request.Both)
                {
                    query = query.Where(x => x.IsAvalaibleForExecution == request.Status);
                }

                // Retrieve the filtered entities
                var entities = await query.AsNoTracking().ToListAsync();

                // Map entities to DTOs and return a successful response
                return ServiceResponse<List<FileUploadDto>>.ReturnResultWith200(_mapper.Map<List<FileUploadDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all FileUploads: {e.Message}");
                return ServiceResponse<List<FileUploadDto>>.Return500(e, "Failed to get all FileUploads");
            }
        }
    }
}
