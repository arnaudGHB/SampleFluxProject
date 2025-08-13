using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FeeMediaR.FileUploadP.Commands;
using CBS.TransactionManagement.Repository.FileUploadP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileUploadMediaR.FileUploadP.Handlers
{
    /// <summary>
    /// Handles the command to delete a FileUpload based on DeleteFileUploadCommand.
    /// </summary>
    public class DeleteFileUploadHandler : IRequestHandler<DeleteFileUploadCommand, ServiceResponse<bool>>
    {
        private readonly IFileUploadRepository _fileUploadRepository; // Repository for accessing FileUpload data.
        private readonly ILogger<DeleteFileUploadHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow; // UnitOfWork for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information for auditing actions.

        /// <summary>
        /// Constructor for initializing the DeleteFileUploadHandler.
        /// </summary>
        /// <param name="fileUploadRepository">Repository for FileUpload data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">UnitOfWork for managing transactions.</param>
        /// <param name="userInfoToken">User info for logging and auditing.</param>
        public DeleteFileUploadHandler(
            IFileUploadRepository fileUploadRepository,
            IMapper mapper,
            ILogger<DeleteFileUploadHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            UserInfoToken userInfoToken)
        {
            _fileUploadRepository = fileUploadRepository ?? throw new ArgumentNullException(nameof(fileUploadRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
        }

        /// <summary>
        /// Handles the DeleteFileUploadCommand to delete a FileUpload.
        /// </summary>
        /// <param name="request">The DeleteFileUploadCommand containing FileUpload ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteFileUploadCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                // Validate that the request ID is not null or empty.
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    errorMessage = "The FileUpload ID must be provided.";
                    _logger.LogWarning(errorMessage);

                    // Log and audit the validation failure.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.DeleteFileUpload, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return400(errorMessage);
                }

                // Check if the FileUpload entity with the specified ID exists.
                var existingFileUpload = await _fileUploadRepository.FindAsync(request.Id);
                if (existingFileUpload == null)
                {
                    errorMessage = $"FileUpload with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);

                    // Log and audit the "not found" scenario.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.DeleteFileUpload, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the FileUpload entity as deleted.
                existingFileUpload.IsDeleted = true;

                // Save the changes to the database.
                _fileUploadRepository.Update(existingFileUpload);
                await _uow.SaveAsync();

                // Log and audit the successful deletion.
                string successMessage = $"{_userInfoToken.FullName} Successfully marked FileUpload with ID: {request.Id} as deleted.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.DeleteFileUpload, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors.
                errorMessage = $"An error occurred while deleting FileUpload with ID {request.Id}: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the exception.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.DeleteFileUpload, LogLevelInfo.Error);

                return ServiceResponse<bool>.Return500(ex);
            }
        }
    }

}
