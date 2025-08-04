using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Queries;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{



    /// <summary>
    /// Handles the query to retrieve the salary analysis result by its unique ID.
    /// </summary>
    public class GetSalaryAnalysisResultQueryByIdHandler : IRequestHandler<GetSalaryAnalysisResultQueryById, ServiceResponse<SalaryAnalysisResultDto>>
    {
        private readonly ISalaryAnalysisResultRepository _salaryAnalysisResultRepository; // Repository for accessing salary analysis results.
        private readonly IMapper _mapper; // AutoMapper instance for mapping entities to DTOs.
        private readonly ILogger<GetSalaryAnalysisResultQueryByIdHandler> _logger; // Logger for logging actions and errors.

        /// <summary>
        /// Constructor for initializing the <see cref="GetSalaryAnalysisResultQueryByIdHandler"/> class.
        /// </summary>
        /// <param name="salaryAnalysisResultRepository">Repository for accessing salary analysis results.</param>
        /// <param name="mapper">AutoMapper for mapping entities to DTOs.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSalaryAnalysisResultQueryByIdHandler(
            ISalaryAnalysisResultRepository salaryAnalysisResultRepository,
            IMapper mapper,
            ILogger<GetSalaryAnalysisResultQueryByIdHandler> logger)
        {
            _salaryAnalysisResultRepository = salaryAnalysisResultRepository ?? throw new ArgumentNullException(nameof(salaryAnalysisResultRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the query to retrieve the salary analysis result by its unique ID.
        /// </summary>
        /// <param name="request">The query containing the unique ID to retrieve the salary analysis result.</param>
        /// <param name="cancellationToken">Cancellation token for handling request cancellation.</param>
        /// <returns>A service response containing the salary analysis result DTO or an error message.</returns>
        public async Task<ServiceResponse<SalaryAnalysisResultDto>> Handle(GetSalaryAnalysisResultQueryById request, CancellationToken cancellationToken)
        {
            // Generate a unique log reference for tracking the operation.
            string logReference = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Validate that the ID is provided.
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    const string errorMessage = "Id is required to retrieve the salary analysis result.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.GetSalaryAnalysisResult, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return400(errorMessage);
                }

                // Retrieve the salary analysis result by ID from the repository.
                var analysisResult = await _salaryAnalysisResultRepository
                    .FindBy(x => x.Id == request.Id)
                    .Include(x => x.FileUpload) // Include related FileUpload entity for additional context.
                    .Include(x => x.salaryAnalysisResultDetails) // Include related salary analysis details.
                    .AsNoTracking() // Disable tracking for better query performance.
                    .FirstOrDefaultAsync(cancellationToken);

                // Check if the analysis result was found.
                if (analysisResult == null)
                {
                    const string notFoundMessage = "No salary analysis result found for the provided Id.";
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
