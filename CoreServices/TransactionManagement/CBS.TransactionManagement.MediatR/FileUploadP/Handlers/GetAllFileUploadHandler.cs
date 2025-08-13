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
    public class GetAllFileUploadHandler : IRequestHandler<GetAllFileUploadQuery, ServiceResponse<List<FileUploadDto>>>
    {
        private readonly IFileUploadRepository _FileUploadRepository; // Repository for accessing FileUploads data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFileUploadHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllFileUploadQueryHandler.
        /// </summary>
        /// <param name="FileUploadRepository">Repository for FileUploads data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFileUploadHandler(
            IFileUploadRepository FileUploadRepository,
            IMapper mapper, ILogger<GetAllFileUploadHandler> logger, UserInfoToken userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _FileUploadRepository = FileUploadRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken=userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllFileUploadQuery to retrieve all FileUploads.
        /// </summary>
        /// <param name="request">The GetAllFileUploadQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FileUploadDto>>> Handle(GetAllFileUploadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all FileUploads entities from the repository
                var entities = await _FileUploadRepository.FindBy(x => !x.IsDeleted && x.FileCategory==request.FileCategory).AsNoTracking().ToListAsync();
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
