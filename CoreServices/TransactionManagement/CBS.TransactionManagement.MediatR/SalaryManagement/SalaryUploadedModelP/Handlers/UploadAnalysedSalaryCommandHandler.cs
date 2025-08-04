using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{
    /// <summary>
    /// Handles the command to add a new ReopenFeeParameter.
    /// </summary>
    public class UploadAnalysedSalaryCommandHandler : IRequestHandler<UploadAnalysedSalaryCommand, ServiceResponse<bool>>
    {
        private readonly ISalaryExecutedRepository _salaryExtractRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UploadAnalysedSalaryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddSalaryUploadModelCommandHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public UploadAnalysedSalaryCommandHandler(
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<UploadAnalysedSalaryCommandHandler> logger,
            IMediator mediator = null,
            ISalaryExecutedRepository salaryExtractRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mediator = mediator;
            _salaryExtractRepository = salaryExtractRepository;
        }

        /// <summary>
        /// Handles the UploadAnalysedSalaryCommand to extract and process salary files.
        /// </summary>
        /// <param name="request">The UploadAnalysedSalaryCommand containing salary file data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response indicating the success or failure of the operation.</returns>
        public async Task<ServiceResponse<bool>> Handle(UploadAnalysedSalaryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Log the start of salary file extraction
                string startMessage = $"Initiating salary file extraction for branch {_userInfoToken.BranchName} by {_userInfoToken.FullName}.";
                _logger.LogInformation(startMessage);
                await BaseUtilities.LogAndAuditAsync(startMessage, request, HttpStatusCodeEnum.OK, LogAction.AnalysedSalaryFileUpload, LogLevelInfo.Information);

                // Extract salary file data into the database
                var salaryExtractDtos = await _salaryExtractRepository.ExtractExcelDataToDatabase(
                    request.AttachedFiles,
                    _userInfoToken.BranchID,
                    _userInfoToken.BranchName,
                    _userInfoToken.BranchCode,
                    request.UploadFileId
                );

                // Log success and return response
                string successMessage = $"Salary file successfully extracted and processed for branch {_userInfoToken.BranchName} by {_userInfoToken.FullName}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AnalysedSalaryFileUpload, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception e)
            {
                // Log error and return 500 response
                string errorMessage = $"Error extracting salary file for branch {_userInfoToken.BranchName} by {_userInfoToken.FullName}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AnalysedSalaryFileUpload, LogLevelInfo.Error);

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }


    }

}
