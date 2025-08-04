using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Commands;
using CBS.NLoan.Repository.FileDownloadInfoP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class InitiateDownloadInfoCommandHandler : IRequestHandler<InitiateDownloadInfoCommand, ServiceResponse<FileDownloadInfoDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<InitiateDownloadInfoCommandHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly ILoanRepository _accountRepository;

        public InitiateDownloadInfoCommandHandler(
            IMapper mapper,
            ILogger<InitiateDownloadInfoCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanRepository accountRepository = null,
            IMediator mediator = null)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _accountRepository = accountRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the request to initiate download information.
        /// </summary>
        /// <param name="request">The command request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Service response containing file download information.</returns>
        public async Task<ServiceResponse<FileDownloadInfoDto>> Handle(InitiateDownloadInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                FileDownloadInfoDto infoDto;
                if (request.IsByBranch)
                {
                    // Retrieve branch information
                    var branch = await GetBranch(request.BranchId);
                    // Export customer balances by branch ID
                    infoDto = await _accountRepository.ExportLoansAsync(branch.id,request.StartDate,request.EndDate,branch.name, branch.branchCode, request.QueryParameter,request.IsUnpaidOnly);
                }
                else
                {
                    // Export customer balances for all ordinary accounts
                    infoDto = await _accountRepository.ExportLoansAsync(request.StartDate, request.EndDate,"N/A", "N/A", request.QueryParameter, request.IsUnpaidOnly);
                }
                infoDto.UserName = request.FullName;
                infoDto.UserId = request.UserId;
                infoDto.BranchName = request.BranchName;
                infoDto.BranchId = request.BranchId;
                // Create file download information record in the database
                await CreateFileDownloadInfo(infoDto);

                // Map and return the file download information
                var fileDownloadInfoDto = _mapper.Map<FileDownloadInfoDto>(infoDto);
                return ServiceResponse<FileDownloadInfoDto>.ReturnResultWith200(fileDownloadInfoDto,"File initialization completed.");
            }
            catch (Exception e)
            {
                // Log and handle exceptions
                var errorMessage = $"Error occurred while saving FileDownloadInfo: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FileDownloadInfoDto>.Return500(e);
            }
        }

        /// <summary>
        /// Retrieves branch information by branch ID.
        /// </summary>
        /// <param name="branchId">The branch ID.</param>
        /// <returns>The branch data transfer object.</returns>
        private async Task<BranchDto> GetBranch(string branchId)
        {
            var branchCommandQuery = new BranchPICallCommand { BranchId = branchId };
            var branchResponse = await _mediator.Send(branchCommandQuery);

            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return branchResponse.Data;
        }

        /// <summary>
        /// Creates a file download information record.
        /// </summary>
        /// <param name="infoDto">The file download information data transfer object.</param>
        private async Task CreateFileDownloadInfo(FileDownloadInfoDto infoDto)
        {
            var addFileCommand = new AddFileDownloadInfoCommand
            {
                DownloadPath = infoDto.DownloadPath,
                Extension = infoDto.Extension,
                FileName = infoDto.FileName,
                FileType = infoDto.FileType,
                FullPath = infoDto.FullPath,
                Size = infoDto.Size,
                TransactionType = infoDto.TransactionType,
                BranchId = infoDto.BranchId,
                BranchName = infoDto.BranchName,
                UserId = infoDto.UserId,
                FullName = infoDto.UserName
            };

            var response = await _mediator.Send(addFileCommand);

            if (response.StatusCode != 200)
            {
                var errorMessage = $"Failed creating file download info: {response.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }

}
