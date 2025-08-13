using AutoMapper;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FileUploadP.Queries;
using CBS.TransactionManagement.Repository.FileUploadP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileUploadMediaR.FileUploadP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetFileUploadHandler : IRequestHandler<GetFileUploadQuery, ServiceResponse<FileUploadDto>>
    {
        private readonly IFileUploadRepository _FileUploadRepository; // Repository for accessing FileUpload data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFileUploadHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetFileUploadQueryHandler.
        /// </summary>
        /// <param name="FileUploadRepository">Repository for FileUpload data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetFileUploadHandler(
            IFileUploadRepository FileUploadRepository,
            IMapper mapper,
            ILogger<GetFileUploadHandler> logger)
        {
            _FileUploadRepository = FileUploadRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetFileUploadQuery to retrieve a specific FileUpload.
        /// </summary>
        /// <param name="request">The GetFileUploadQuery containing FileUpload ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FileUploadDto>> Handle(GetFileUploadQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the FileUpload entity with the specified ID from the repository
                var entity = await _FileUploadRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the FileUpload entity to FileUploadDto and return it with a success response
                    var FileUploadDto = _mapper.Map<FileUploadDto>(entity);
                    return ServiceResponse<FileUploadDto>.ReturnResultWith200(FileUploadDto);
                }
                else
                {
                    // If the FileUpload entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("FileUpload not found.");
                    return ServiceResponse<FileUploadDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting FileUpload: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<FileUploadDto>.Return500(e);
            }
        }
    }

}
