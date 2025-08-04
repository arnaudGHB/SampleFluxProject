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
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{
    /// <summary>
    /// Handles the command to add a new ReopenFeeParameter.
    /// </summary>
    public class AddSalaryFileUploadCommandHandler : IRequestHandler<AddSalaryFileUploadCommand, ServiceResponse<List<SalaryExtractDto>>>
    {
        private readonly ISalaryProcessingRepository _salaryExtractRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddSalaryFileUploadCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddSalaryUploadModelCommandHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddSalaryFileUploadCommandHandler(
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddSalaryFileUploadCommandHandler> logger,
            IMediator mediator = null,
            ISalaryProcessingRepository salaryExtractRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mediator = mediator;
            _salaryExtractRepository = salaryExtractRepository;
        }

        /// <summary>
        /// Handles the AddStandingOrderCommand to add a new ReopenFeeParameter.
        /// </summary>
        /// <param name="request">The AddStandingOrderCommand containing ReopenFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SalaryExtractDto>>> Handle(AddSalaryFileUploadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string fileCode = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"F{_userInfoToken.BranchCode}");
                var salaryExtractDtos = await _salaryExtractRepository.ExtractExcelDataToDatabase(request.File, _userInfoToken.BranchID, _userInfoToken.BranchName, _userInfoToken.BranchCode, fileCode);
                string message = $"Salary file successfully extracted for branch {_userInfoToken.BranchName} by {_userInfoToken.FullName}";
                await LogAndAuditInfo(request, message, 200);
                var salaryExtracts = _mapper.Map<List<SalaryExtractDto>>(salaryExtractDtos);
                return ServiceResponse<List<SalaryExtractDto>>.ReturnResultWith200(salaryExtracts);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error extracting salary file for {_userInfoToken.BranchName} by {_userInfoToken.FullName}: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<List<SalaryExtractDto>>.Return500(errorMessage);
            }
        }
        private async Task LogAndAuditError(AddSalaryFileUploadCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }
        private async Task LogAndAuditInfo(AddSalaryFileUploadCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }
    }

}
