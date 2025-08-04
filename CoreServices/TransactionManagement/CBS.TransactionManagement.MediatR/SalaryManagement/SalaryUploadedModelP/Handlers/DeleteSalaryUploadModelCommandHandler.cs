using MediatR;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Handlers
{

    /// <summary>
    /// Handles the command to delete a SalaryUploadModel.
    /// </summary>
    public class DeleteSalaryUploadModelCommandHandler : IRequestHandler<DeleteSalaryUploadModelCommand, ServiceResponse<bool>>
    {
        private readonly ISalaryUploadModelRepository _salaryUploadModelRepository;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteSalaryUploadModelCommandHandler.
        /// </summary>
        public DeleteSalaryUploadModelCommandHandler(
            ISalaryUploadModelRepository salaryUploadModelRepository,
            UserInfoToken userInfoToken)
        {
            _salaryUploadModelRepository = salaryUploadModelRepository ?? throw new ArgumentNullException(nameof(salaryUploadModelRepository));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
        }

        /// <summary>
        /// Handles the DeleteSalaryUploadModelCommand to delete a SalaryUploadModel by its ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(DeleteSalaryUploadModelCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FileUploadId))
            {
                const string errorMessage = "Invalid delete request: ID is missing or empty.";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.SalaryUpload,
                    LogLevelInfo.Error
                );
                return ServiceResponse<bool>.Return500(errorMessage);
            }

            try
            {
              
                await _salaryUploadModelRepository.RemoveSalaryUploadModelDataForFile(request.FileUploadId);
                string successMessage = $"SalaryUploadModel with ID {request.FileUploadId} successfully deleted by {_userInfoToken.FullName}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.SalaryUpload,
                    LogLevelInfo.Information
                );

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error deleting SalaryUploadModel with ID {request.FileUploadId}: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.SalaryUpload,
                    LogLevelInfo.Error
                );
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
