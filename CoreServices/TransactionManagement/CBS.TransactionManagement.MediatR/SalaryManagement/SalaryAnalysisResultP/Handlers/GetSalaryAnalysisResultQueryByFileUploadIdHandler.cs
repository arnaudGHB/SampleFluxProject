using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Queries;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{


    /// <summary>
    /// Handles the query to retrieve the salary analysis result by FileUploadId.
    /// </summary>
    public class GetSalaryAnalysisResultQueryByFileUploadIdHandler : IRequestHandler<GetSalaryAnalysisResultQueryByFileUploadId, ServiceResponse<SalaryAnalysisResultDto>>
    {
        private readonly ISalaryAnalysisResultRepository _salaryAnalysisResultRepository; // Repository for accessing salary analysis results.
        private readonly IMapper _mapper; // AutoMapper instance for mapping entities to DTOs.
        private readonly ILogger<GetSalaryAnalysisResultQueryByFileUploadIdHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken; // Repository for accessing salary analysis results.

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSalaryAnalysisResultQueryByFileUploadIdHandler"/> class.
        /// </summary>
        /// <param name="salaryAnalysisResultRepository">Repository for accessing salary analysis results.</param>
        /// <param name="mapper">AutoMapper for mapping entities to DTOs.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSalaryAnalysisResultQueryByFileUploadIdHandler(
            ISalaryAnalysisResultRepository salaryAnalysisResultRepository,
            IMapper mapper,
            ILogger<GetSalaryAnalysisResultQueryByFileUploadIdHandler> logger,
            UserInfoToken userInfoToken)
        {
            _salaryAnalysisResultRepository = salaryAnalysisResultRepository ?? throw new ArgumentNullException(nameof(salaryAnalysisResultRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userInfoToken=userInfoToken;
        }

        /// <summary>
        /// Handles the query to retrieve the salary analysis result for the specified FileUploadId.
        /// </summary>
        /// <param name="request">The query containing the FileUploadId to retrieve the salary analysis result.</param>
        /// <param name="cancellationToken">Cancellation token for handling request cancellation.</param>
        /// <returns>A service response containing the salary analysis result DTO or an error message.</returns>
        public async Task<ServiceResponse<SalaryAnalysisResultDto>> Handle(GetSalaryAnalysisResultQueryByFileUploadId request, CancellationToken cancellationToken)
        {
            // Generate a unique log reference for tracking the operation.
            string logReference = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Validate the FileUploadId is provided.
                if (string.IsNullOrWhiteSpace(request.FileUploadId))
                {
                    const string errorMessage = "FileUploadId is required to retrieve the salary analysis result.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.GetSalaryAnalysisResult, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return400(errorMessage);
                }

                // Retrieve the salary analysis result by FileUploadId from the repository.
                var analysisResult = await _salaryAnalysisResultRepository
                    .FindBy(x => x.FileUploadId == request.FileUploadId && x.BranchId==_userInfoToken.BranchID)
                    .Include(x => x.FileUpload) // Include related FileUpload entity for additional context.
                    .Include(x => x.salaryAnalysisResultDetails) // Include related salary analysis details.
                    .AsNoTracking() // Disable tracking for better query performance.
                    .FirstOrDefaultAsync(cancellationToken);

                // Check if the analysis result was found.
                if (analysisResult == null)
                {
                    const string notFoundMessage = "No salary analysis result found for the provided FileUploadId.";
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GetSalaryAnalysisResult, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return404(notFoundMessage);
                }

                // Map the analysis result to the DTO using AutoMapper.
                var resultDto = _mapper.Map<SalaryAnalysisResultDto>(analysisResult);

                // Log and audit the successful retrieval of the analysis result.
                string successMessage = "Salary analysis result retrieved successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.GetSalaryAnalysisResult, LogLevelInfo.Information, logReference);

                // Return the mapped DTO in the service response.
                return ServiceResponse<SalaryAnalysisResultDto>.ReturnResultWith200(resultDto, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit any unexpected exceptions during the retrieval.
                string errorMessage = $"An error occurred while retrieving the salary analysis result: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetSalaryAnalysisResult, LogLevelInfo.Error, logReference);
                return ServiceResponse<SalaryAnalysisResultDto>.Return500(errorMessage);
            }
        }
    }


}
