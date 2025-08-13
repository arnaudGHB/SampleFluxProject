using MediatR;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.MediatR.UtilityServices;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{
    /// <summary>
    /// Handles the command to perform salary analysis based on uploaded salary data.
    /// </summary>
    public class SalaryAnalysisCommandHandler : IRequestHandler<SalaryAnalysisCommand, ServiceResponse<SalaryAnalysisResultDto>>
    {
        private readonly ISalaryAnalysisResultDetailRepository _salaryAnalysisResultDetailRepository; // Repository for salary analysis operations.
        private readonly UserInfoToken _userInfoToken; // Holds information about the current user for context.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Utility services for fetching branch, members, and loans.
        private readonly ISalaryUploadModelRepository _salaryUploadModelRepository; // Repository for accessing salary upload data.

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryAnalysisCommandHandler"/> class.
        /// </summary>
        /// <param name="salaryAnalysisResultDetailRepository">Repository for performing salary analysis operations.</param>
        /// <param name="userInfoToken">User information for auditing and context.</param>
        /// <param name="utilityServicesRepository">Repository for utility operations (e.g., fetching members, loans).</param>
        /// <param name="salaryUploadModelRepository">Repository for accessing salary upload data.</param>
        public SalaryAnalysisCommandHandler(
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
        public async Task<ServiceResponse<SalaryAnalysisResultDto>> Handle(SalaryAnalysisCommand request, CancellationToken cancellationToken)
        {
            // Generate a unique log reference for traceability.
            string logReference = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Validate that the FileUploadId is provided.
                if (string.IsNullOrWhiteSpace(request.FileUploadId))
                {
                    const string errorMessage = "FileUploadId is required for salary analysis.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.SalaryAnalysis, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return400(errorMessage);
                }

                // Fetch branch details using the user's branch ID.
                var branch = await _utilityServicesRepository.GetBranch(_userInfoToken.BranchID);
                if (branch == null)
                {
                    string errorMessage = $"Branch with ID {_userInfoToken.BranchID} not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.SalaryAnalysis, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return404(errorMessage);
                }

                // Fetch salary upload models for the given FileUploadId
                var salaryUploadModels = await _salaryUploadModelRepository
                    .FindBy(x => x.FileUploadId == request.FileUploadId)
                    .ToListAsync(cancellationToken);

                if (!salaryUploadModels.Any())
                {
                    const string errorMessage = "No salary data found for the provided FileUploadId.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.SalaryAnalysis, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return404(errorMessage);
                }

                // Fetch member data associated with the branch.
                var members = await _utilityServicesRepository.GetMembersWithMatriculeByBranchQuery(branch.id);
                if (members == null || !members.Any())
                {
                    const string errorMessage = "No members found matching the provided matricules.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.SalaryAnalysis, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<SalaryAnalysisResultDto>.Return404(errorMessage);
                }

                // Join salary models with members based on Matricule.
                var jointResultSalaryModels = (from salary in salaryUploadModels
                                               join member in members on salary.Matricule equals member.Matricule
                                               select new SalaryUploadModel
                                               {
                                                   Id = salary.Id,
                                                   CustomerId=member.CustomerId,
                                                   SalaryCode = salary.SalaryCode,
                                                   Matricule = salary.Matricule,
                                                   Surname = salary.Surname,
                                                   Name = salary.Name,
                                                   LaisonBankAccountNumber = salary.LaisonBankAccountNumber,
                                                   NetSalary = salary.NetSalary,
                                                   BranchId = salary.BranchId,
                                                   BranchName = branch.name,
                                                   BranchCode = branch.branchCode,
                                                   FileUploadId = salary.FileUploadId,
                                                   Date = salary.Date,
                                                   UploadedBy = salary.UploadedBy,
                                                   SalaryType = salary.SalaryType,
                                                   FileUpload = salary.FileUpload,
                                               }).ToList();

                // Extract members matched in the join operation.
                var newMembers = (from salary in jointResultSalaryModels
                                  join member in members on salary.Matricule equals member.Matricule
                                  select member).ToList();

                // Extract customer IDs from the filtered members (newMembers).
                var customerIds = newMembers.Select(m => m.CustomerId).ToList();

                // Fetch active loans for the filtered members.
                var loans = await _utilityServicesRepository.GetOpenLoansByBranchQuery(branch.id) ?? new List<LightLoanDto>();

                if (!loans.Any())
                {
                    string infoMessage = "No active loans found for the filtered members.";
                    await BaseUtilities.LogAndAuditAsync(infoMessage, request, HttpStatusCodeEnum.OK, LogAction.SalaryAnalysis, LogLevelInfo.Information, logReference);
                }

                var loanRepaymentOrder = await _utilityServicesRepository.GetSalaryLoanRepaymentOrder();
                if (loanRepaymentOrder == null)
                {
                    string infoMessage = "No loan repayment order found.";
                    await BaseUtilities.LogAndAuditAsync(infoMessage, request, HttpStatusCodeEnum.OK, LogAction.SalaryAnalysis, LogLevelInfo.Information, logReference);
                }
                // Perform salary analysis using the repository.
                var analysisResult = await _salaryAnalysisResultDetailRepository.AnalyzeSalaryFileAsync(
                    request.FileUploadId,
                    loanRepaymentOrder,
                    branch,
                    jointResultSalaryModels,
                    loans,
                    newMembers.ToList() // Pass the filtered members to the analysis.
                );

                // Log a success message after completing the salary analysis.
                string successMessage = "Salary analysis completed successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.SalaryAnalysis, LogLevelInfo.Information, logReference);

                return ServiceResponse<SalaryAnalysisResultDto>.ReturnResultWith200(analysisResult, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit any unexpected exceptions during salary analysis.
                string errorMessage = $"An error occurred during salary analysis: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryAnalysis, LogLevelInfo.Error, logReference);
                return ServiceResponse<SalaryAnalysisResultDto>.Return500(errorMessage);
            }
        }
    }


}
