using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.ReopenFeeParameterP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Handlers
{
    /// <summary>
    /// Handles the command to process and save a new SalaryUploadModel.
    /// </summary>
    public class AddSalaryUploadModelCommandHandler : IRequestHandler<AddSalaryUploadModelCommand, ServiceResponse<SalaryUploadModelSummaryDto>>
    {
        private readonly ISalaryUploadModelRepository _salaryExtractRepository; // Repository for salary upload data access.
        private readonly IMapper _mapper; // AutoMapper for mapping between objects.
        private readonly UserInfoToken _userInfoToken; // Contains information about the current user.
        private readonly IAPIUtilityServicesRepository _aPIUtilityServicesRepository; // Repository for salary upload data access.
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSalaryUploadModelCommandHandler"/> class.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">User information for logging and auditing.</param>
        /// <param name="salaryExtractRepository">Repository for accessing salary upload data.</param>
        public AddSalaryUploadModelCommandHandler(
            IMapper mapper,
            UserInfoToken userInfoToken,
            ISalaryUploadModelRepository salaryExtractRepository,
            IAPIUtilityServicesRepository aPIUtilityServicesRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _salaryExtractRepository = salaryExtractRepository ?? throw new ArgumentNullException(nameof(salaryExtractRepository));
            _aPIUtilityServicesRepository=aPIUtilityServicesRepository;
        }

        /// <summary>
        /// Processes the uploaded salary file, extracts data, and returns a summary of the operation.
        /// </summary>
        /// <param name="request">The command containing the uploaded salary file.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing the result of the operation.</returns>
        public async Task<ServiceResponse<SalaryUploadModelSummaryDto>> Handle(AddSalaryUploadModelCommand request, CancellationToken cancellationToken)
        {
            // Validate the input request.
            if (request?.File == null)
            {
                const string errorMessage = "Invalid salary upload request: AttachedFiles is missing.";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.SalaryUpload,
                    LogLevelInfo.Error
                );
                return ServiceResponse<SalaryUploadModelSummaryDto>.Return500(errorMessage);
            }

            try
            {
                // Generate a unique file code for tracking the upload operation.
                string fileCode = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"F{_userInfoToken.BranchCode}");
                var members = await _aPIUtilityServicesRepository.GetAllMembers();
                var branches = await _aPIUtilityServicesRepository.GetBranches();
                
                // Extract data from the uploaded file and save it to the database.
                var salaryExtractDtos = await _salaryExtractRepository.ExtractExcelDataToDatabase(request.File,request.SalaryType, members,branches);
                if (salaryExtractDtos == null)
                {
                    const string errorMessage = "No data extracted from the uploaded file.";
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.InternalServerError,
                        LogAction.SalaryUpload,
                        LogLevelInfo.Error
                    );
                    return ServiceResponse<SalaryUploadModelSummaryDto>.Return500(errorMessage);
                }

                // Log and audit successful file extraction.
                string successMessage = $"Salary file successfully extracted for branch {_userInfoToken.BranchName} by {_userInfoToken.FullName}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.SalaryUpload,
                    LogLevelInfo.Information
                );

                // Map the extracted data to a summary DTO and return it in the response.
                var salaryExtracts = _mapper.Map<SalaryUploadModelSummaryDto>(salaryExtractDtos);
                return ServiceResponse<SalaryUploadModelSummaryDto>.ReturnResultWith200(salaryExtracts);
            }
            catch (Exception ex)
            {
                // Log and audit any errors that occurred during the operation.
                string errorMessage = $"Error extracting salary file for {_userInfoToken.BranchName} by {_userInfoToken.FullName}: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.SalaryUpload,
                    LogLevelInfo.Error
                );
                return ServiceResponse<SalaryUploadModelSummaryDto>.Return500(errorMessage);
            }
        }
    }

}
