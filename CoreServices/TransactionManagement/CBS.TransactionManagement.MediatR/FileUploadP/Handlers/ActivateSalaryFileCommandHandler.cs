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
    /// Handles the command to activate or deactivate a salary file based on the given status.
    /// </summary>
    public class ActivateSalaryFileCommandHandler : IRequestHandler<ActivateSalaryFileCommand, ServiceResponse<bool>>
    {
        private readonly IFileUploadRepository _fileUploadRepository; // Repository for accessing FileUpload data.
        private readonly ILogger<ActivateSalaryFileCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User info for auditing.

        /// <summary>
        /// Constructor for initializing the ActivateSalaryFileCommandHandler.
        /// </summary>
        /// <param name="fileUploadRepository">Repository for FileUpload data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        /// <param name="userInfoToken">User info token for logging and auditing.</param>
        public ActivateSalaryFileCommandHandler(
            IFileUploadRepository fileUploadRepository,
            IMapper mapper,
            ILogger<ActivateSalaryFileCommandHandler> logger,
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
        /// Handles the ActivateSalaryFileCommand to activate or deactivate a salary file.
        /// </summary>
        /// <param name="request">The ActivateSalaryFileCommand containing the file ID and status to update.</param>
        /// <param name="cancellationToken">A cancellation token for handling task cancellation.</param>
        /// <returns>Service response indicating success or failure of the operation.</returns>
        public async Task<ServiceResponse<bool>> Handle(ActivateSalaryFileCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                string Status = "De-activated";
                if (request.Status)
                {
                    Status = "Activated";
                }
                // Validate that the request ID is not null or empty.
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    errorMessage = "The FileUpload ID must be provided.";
                    _logger.LogWarning(errorMessage);

                    // Log and audit the error.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.ActivateDeactivateSalaryFile, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return400(errorMessage);
                }

                // Check if the FileUpload entity with the specified ID exists.
                var existingFileUpload = await _fileUploadRepository.FindAsync(request.Id);
                if (existingFileUpload == null)
                {
                    errorMessage = $"FileUpload with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);

                    // Log and audit the error.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ActivateDeactivateSalaryFile, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Update the status of the FileUpload entity.
                existingFileUpload.IsAvalaibleForExecution = request.Status;

                // Save the changes to the database.
                _fileUploadRepository.Update(existingFileUpload);
                await _uow.SaveAsync();

                // Log and audit the successful update.
                string successMessage = $"{_userInfoToken.FullName} Successfully {Status} salary file : {existingFileUpload.FileName}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ActivateDeactivateSalaryFile, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors.
                errorMessage = $"An error occurred while {_userInfoToken.FullName} was updating the status of FileUpload with ID {request.Id}: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the exception.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ActivateDeactivateSalaryFile, LogLevelInfo.Error);

                return ServiceResponse<bool>.Return500(ex);
            }
        }
    }

}
