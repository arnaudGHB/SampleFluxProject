using AutoMapper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Entity.FileDownloadInfoP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FileDownloadInfoP.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class InitiateDownloadInfoCommandHandler : IRequestHandler<InitiateDownloadInfoCommand, ServiceResponse<FileDownloadInfoDto>>
    {
        private readonly IFileDownloadInfoRepository _fileDownloadInfoRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<InitiateDownloadInfoCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IAccountRepository _accountRepository;
        private readonly UserInfoToken _userInfoToken; // Contains user information and authentication token.

        public InitiateDownloadInfoCommandHandler(
            IFileDownloadInfoRepository fileDownloadInfoRepository,
            IMapper mapper,
            ILogger<InitiateDownloadInfoCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IAccountRepository accountRepository = null,
            IMediator mediator = null,
            UserInfoToken userInfoToken = null)
        {
            _fileDownloadInfoRepository = fileDownloadInfoRepository ?? throw new ArgumentNullException(nameof(fileDownloadInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _accountRepository = accountRepository;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
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
                var branch = await GetBranch(request.BranchId);
                infoDto = await _accountRepository.ExportMemberAccountSummaryAsync(branch, request.IsByBranch);
                infoDto.UserName = _userInfoToken.FullName;
                infoDto.UserId = _userInfoToken.Id;
                infoDto.BranchName = branch.name;
                infoDto.BranchId = request.BranchId;
                infoDto.DateInitiated = BaseUtilities.UtcNowToDoualaTime();
                // Create file download information record in the database
                await CreateFileDownloadInfo(infoDto);

                // Map and return the file download information
                var fileDownloadInfoDto = _mapper.Map<FileDownloadInfoDto>(infoDto);
                return ServiceResponse<FileDownloadInfoDto>.ReturnResultWith200(fileDownloadInfoDto);
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
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchId };
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
