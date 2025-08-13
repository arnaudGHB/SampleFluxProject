using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Entity.FileDownloadInfoP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FileDownloadInfoP.Commands;
using CBS.TransactionManagement.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddFileDownloadInfoHandler : IRequestHandler<AddFileDownloadInfoCommand, ServiceResponse<FileDownloadInfoDto>>
    {
        private readonly IFileDownloadInfoRepository _FileDownloadInfoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddFileDownloadInfoHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public AddFileDownloadInfoHandler(
            IFileDownloadInfoRepository FileDownloadInfoRepository,
            IMapper mapper,
            ILogger<AddFileDownloadInfoHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            UserInfoToken userInfoToken = null)
        {
            _FileDownloadInfoRepository = FileDownloadInfoRepository ?? throw new ArgumentNullException(nameof(FileDownloadInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<FileDownloadInfoDto>> Handle(AddFileDownloadInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var FileDownloadInfoEntity = _mapper.Map<FileDownloadInfo>(request);
                FileDownloadInfoEntity.Id = BaseUtilities.GenerateUniqueNumber();
                FileDownloadInfoEntity.UserName = request.FullName;
                FileDownloadInfoEntity.BranchName = request.BranchName;
                FileDownloadInfoEntity.BranchId = request.BranchId;
                FileDownloadInfoEntity.CreatedBy = request.UserId;
                FileDownloadInfoEntity.DateInitiated = BaseUtilities.UtcNowToDoualaTime();
                _FileDownloadInfoRepository.Add(FileDownloadInfoEntity);
                await _uow.SaveAsync();
                var FileDownloadInfoDto = _mapper.Map<FileDownloadInfoDto>(FileDownloadInfoEntity);
                return ServiceResponse<FileDownloadInfoDto>.ReturnResultWith200(FileDownloadInfoDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving FileDownloadInfo: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FileDownloadInfoDto>.Return500(e);
            }
        }
    }

}
