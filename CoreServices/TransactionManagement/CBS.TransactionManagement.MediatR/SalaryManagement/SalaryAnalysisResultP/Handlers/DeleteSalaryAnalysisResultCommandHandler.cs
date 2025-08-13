using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{


    /// <summary>
    /// Handles the command to delete a salary analysis result.
    /// </summary>
    public class DeleteSalaryAnalysisResultCommandHandler : IRequestHandler<DeleteSalaryAnalysisResultCommand, ServiceResponse<bool>>
    {
        private readonly ISalaryAnalysisResultRepository _salaryAnalysisResultRepository; // Repository for accessing salary analysis results.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for transactional database operations.
        private readonly ILogger<DeleteSalaryAnalysisResultCommandHandler> _logger; // Logger for logging actions and errors.

        /// <summary>
        /// Constructor for initializing the <see cref="DeleteSalaryAnalysisResultCommandHandler"/> class.
        /// </summary>
        /// <param name="salaryAnalysisResultRepository">Repository for accessing salary analysis results.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for transactional database operations.</param>
        public DeleteSalaryAnalysisResultCommandHandler(
            ISalaryAnalysisResultRepository salaryAnalysisResultRepository,
            ILogger<DeleteSalaryAnalysisResultCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _salaryAnalysisResultRepository = salaryAnalysisResultRepository ?? throw new ArgumentNullException(nameof(salaryAnalysisResultRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        /// <summary>
        /// Handles the command to delete a salary analysis result.
        /// </summary>
        /// <param name="request">The command containing the unique ID of the salary analysis result to delete.</param>
        /// <param name="cancellationToken">Cancellation token for handling request cancellation.</param>
        /// <returns>A service response indicating the success or failure of the deletion.</returns>
        public async Task<ServiceResponse<bool>> Handle(DeleteSalaryAnalysisResultCommand request, CancellationToken cancellationToken)
        {
            // Generate a unique log reference for tracking the operation.
            string logReference = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Validate that the ID is provided.
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    const string errorMessage = "Id is required to delete the salary analysis result.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.DeleteSalaryAnalysisResult, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return400(errorMessage);
                }

                // Find the salary analysis result by ID.
                var analysisResult = await _salaryAnalysisResultRepository
                    .FindBy(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                // Check if the analysis result exists.
                if (analysisResult == null)
                {
                    const string notFoundMessage = "No salary analysis result found for the provided Id.";
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.DeleteSalaryAnalysisResult, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return404(notFoundMessage);
                }

                // Remove the analysis result from the repository.
                _salaryAnalysisResultRepository.Remove(analysisResult);
                await _uow.SaveAsync();

                // Log and audit the successful deletion of the salary analysis result.
                string successMessage = $"Successfully deleted salary analysis result with Id: {request.Id}";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.DeleteSalaryAnalysisResult, LogLevelInfo.Information, logReference);

                // Return a successful service response.
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit any unexpected exceptions during deletion.
                string errorMessage = $"An error occurred while deleting the salary analysis result: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.DeleteSalaryAnalysisResult, LogLevelInfo.Error, logReference);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }


}
