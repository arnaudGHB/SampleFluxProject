using AutoMapper;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Handlers
{
    /// <summary>
    /// Handles the query to get SalaryUploadModels by date, file upload ID, or salary code.
    /// </summary>
    public class GetSalaryUploadModelQueryHandler : IRequestHandler<GetSalaryUploadModelQuery, ServiceResponse<List<SalaryUploadModelDto>>>
    {
        private readonly ISalaryUploadModelRepository _salaryUploadModelRepository; // Repository for accessing SalaryUploadModel data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GetSalaryUploadModelQueryHandler.
        /// </summary>
        public GetSalaryUploadModelQueryHandler(
            ISalaryUploadModelRepository salaryUploadModelRepository,
            IMapper mapper)
        {
            _salaryUploadModelRepository = salaryUploadModelRepository ?? throw new ArgumentNullException(nameof(salaryUploadModelRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Handles the query to retrieve SalaryUploadModels based on filters.
        /// </summary>
        /// <param name="request">The query containing filter criteria (Date, FileUploadId, or SalaryCode).</param>
        /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
        /// <returns>A service response containing a list of SalaryUploadModel DTOs.</returns>
        public async Task<ServiceResponse<List<SalaryUploadModelDto>>> Handle(GetSalaryUploadModelQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate that at least one filter is provided.
                if (request.Date == null && string.IsNullOrWhiteSpace(request.FileUploadId) && string.IsNullOrWhiteSpace(request.SalaryCode))
                {
                    const string errorMessage = "At least one filter (Date, FileUploadId, or SalaryCode) must be provided.";
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.QuerySalaryUpload,
                        LogLevelInfo.Warning
                    );
                    return ServiceResponse<List<SalaryUploadModelDto>>.Return400(errorMessage);
                }

                // Retrieve salary upload models based on the filter criteria.
                var salaryUploadModels = await _salaryUploadModelRepository
                 .FindBy(x =>
                     (request.Date == null || request.Date == default(DateTime) || x.Date.Date == request.Date.Value.Date) &&
                     (string.IsNullOrWhiteSpace(request.FileUploadId) || x.FileUploadId == request.FileUploadId) &&
                     (string.IsNullOrWhiteSpace(request.SalaryCode) || x.SalaryCode == request.SalaryCode))
                 .Include(x => x.FileUpload) // Include related FileUpload entity for additional data.
                 .AsNoTracking() // Ensure the query does not track entities in EF Core.
                 .ToListAsync(cancellationToken);



                // Check if no records were found.
                if (salaryUploadModels == null || !salaryUploadModels.Any())
                {
                    const string notFoundMessage = "No salary upload records found for the given criteria.";
                    await BaseUtilities.LogAndAuditAsync(
                        notFoundMessage,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.QuerySalaryUpload,
                        LogLevelInfo.Information
                    );
                    return ServiceResponse<List<SalaryUploadModelDto>>.Return404(notFoundMessage);
                }

                // Map the retrieved entities to DTOs.
                var salaryUploadModelDtos = _mapper.Map<List<SalaryUploadModelDto>>(salaryUploadModels);

                // Log successful retrieval.
                string successMessage = $"Successfully retrieved {salaryUploadModelDtos.Count} salary upload record(s).";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.QuerySalaryUpload,
                    LogLevelInfo.Information
                );

                return ServiceResponse<List<SalaryUploadModelDto>>.ReturnResultWith200(salaryUploadModelDtos);
            }
            catch (Exception ex)
            {
                // Handle and log unexpected errors.
                string errorMessage = $"Error retrieving salary upload records: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.QuerySalaryUpload,
                    LogLevelInfo.Error
                );
                return ServiceResponse<List<SalaryUploadModelDto>>.Return500(errorMessage);
            }
        }
    }

}
