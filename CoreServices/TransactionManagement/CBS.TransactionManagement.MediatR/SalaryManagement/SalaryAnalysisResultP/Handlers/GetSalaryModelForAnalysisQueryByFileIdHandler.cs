using MediatR;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.MediatR.UtilityServices;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Queries;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{
    /// <summary>
    /// Handles the command to perform salary analysis based on uploaded salary data.
    /// </summary>
    public class GetSalaryModelForAnalysisQueryByFileIdHandler : IRequestHandler<GetSalaryModelForAnalysisQueryByFileId, ServiceResponse<List<SalaryUploadModelDto>>>
    {
        private readonly ISalaryAnalysisResultDetailRepository _salaryAnalysisResultDetailRepository; // Repository for salary analysis operations.
        private readonly UserInfoToken _userInfoToken; // Holds information about the current user for context.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Utility services for fetching branch, members, and loans.
        private readonly ISalaryUploadModelRepository _salaryUploadModelRepository; // Repository for accessing salary upload data.

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSalaryModelForAnalysisQueryByFileIdHandler"/> class.
        /// </summary>
        /// <param name="salaryAnalysisResultDetailRepository">Repository for performing salary analysis operations.</param>
        /// <param name="userInfoToken">User information for auditing and context.</param>
        /// <param name="utilityServicesRepository">Repository for utility operations (e.g., fetching members, loans).</param>
        /// <param name="salaryUploadModelRepository">Repository for accessing salary upload data.</param>
        public GetSalaryModelForAnalysisQueryByFileIdHandler(
            ISalaryAnalysisResultDetailRepository salaryAnalysisResultDetailRepository,
            UserInfoToken userInfoToken,
            IAPIUtilityServicesRepository utilityServicesRepository,
            ISalaryUploadModelRepository salaryUploadModelRepository)
        {
            _salaryAnalysisResultDetailRepository = salaryAnalysisResultDetailRepository ?? throw new ArgumentNullException(nameof(salaryAnalysisResultDetailRepository));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _utilityServicesRepository = utilityServicesRepository ?? throw new ArgumentNullException(nameof(utilityServicesRepository));
            _salaryUploadModelRepository = salaryUploadModelRepository ?? throw new ArgumentNullException(nameof(salaryUploadModelRepository));
        }

        /// <summary>
        /// Handles the command to perform salary analysis for the uploaded file.
        /// </summary>
        /// <param name="request">The command containing FileUploadId for salary analysis.</param>
        /// <param name="cancellationToken">Token for handling operation cancellation.</param>
        /// <returns>Returns a service response containing the salary analysis results or an error message.</returns>
        public async Task<ServiceResponse<List<SalaryUploadModelDto>>> Handle(GetSalaryModelForAnalysisQueryByFileId request, CancellationToken cancellationToken)
        {
            // Generate a unique log reference for tracking the operation.
            string logReference = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Validate the FileUploadId is provided.
                if (string.IsNullOrWhiteSpace(request.FileId))
                {
                    const string errorMessage = "FileUploadId is required for salary analysis.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.SalaryAnalysisRetrivalOfSalaryMembers, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<List<SalaryUploadModelDto>>.Return400(errorMessage);
                }

                // Fetch branch information using the user's branch ID.
                var branch = await _utilityServicesRepository.GetBranch(_userInfoToken.BranchID);
                if (branch == null)
                {
                    string errorMessage = $"Branch with ID {_userInfoToken.BranchID} not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.SalaryAnalysisRetrivalOfSalaryMembers, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<List<SalaryUploadModelDto>>.Return404(errorMessage);
                }

                // Fetch salary upload models for the given FileUploadId
                var salaryUploadModels = await _salaryUploadModelRepository
                    .FindBy(x => x.FileUploadId == request.FileId)
                    .ToListAsync(cancellationToken);

                // Check if any salary data was found for the given FileUploadId.
                if (!salaryUploadModels.Any())
                {
                    const string errorMessage = "No salary data found for the provided FileUploadId.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.SalaryAnalysisRetrivalOfSalaryMembers, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<List<SalaryUploadModelDto>>.Return404(errorMessage);
                }

                // Extract matricules (unique employee IDs) from the salary upload models.
                var matricules = salaryUploadModels.Select(s => s.Matricule).Distinct().ToList();

                // Fetch member information for the given matricules.
                var members = await _utilityServicesRepository.GetMembersWithMatriculeByBranchQuery(branch.id);

                // Perform a join between salary upload models and members based on Matricule.
                var jointResultSalaryModelDto = (from salary in salaryUploadModels
                                                 join member in members on salary.Matricule equals member.Matricule
                                                 select new SalaryUploadModelDto
                                                 {
                                                     Id = salary.Id,
                                                     SalaryCode = salary.SalaryCode,
                                                     Matricule = salary.Matricule,
                                                     Surname = salary.Surname,
                                                     Name = salary.Name,
                                                     LaisonBankAccountNumber = salary.LaisonBankAccountNumber,
                                                     NetSalary = salary.NetSalary,
                                                     BranchId = salary.BranchId,
                                                     BranchName = branch.name, // Use the fetched branch name
                                                     BranchCode = branch.branchCode,
                                                     FileUploadId = salary.FileUploadId,
                                                     Date = salary.Date,
                                                     UploadedBy = salary.UploadedBy,
                                                     SalaryType = salary.SalaryType,
                                                     FileUpload = salary.FileUpload,
                                                     SalaryUploadModelSummaryDto = new SalaryUploadModelSummaryDto
                                                     {
                                                         TotalNetSalary = salaryUploadModels.Sum(s => s.NetSalary),
                                                         TotalMembers = salaryUploadModels.Count
                                                     }
                                                 }).ToList();

                // Log a success message after completing the salary analysis.
                string successMessage = $"Salary model of {branch.name} was successfully retrived from the general salay model of {salaryUploadModels.Count}";
                await BaseUtilities.LogAndAuditAsync(successMessage, jointResultSalaryModelDto, HttpStatusCodeEnum.OK, LogAction.SalaryAnalysisRetrivalOfSalaryMembers, LogLevelInfo.Information, logReference);

                // Return the analysis result in the service response.
                return ServiceResponse<List<SalaryUploadModelDto>>.ReturnResultWith200(jointResultSalaryModelDto, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit any unexpected exceptions during salary analysis.
                string errorMessage = $"An error occurred during salary analysis: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryAnalysisRetrivalOfSalaryMembers, LogLevelInfo.Error, logReference);
                return ServiceResponse<List<SalaryUploadModelDto>>.Return500(errorMessage);
            }
        }
    }


}
