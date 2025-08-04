using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.FileDownloadInfoP;
using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Commands;
using CBS.NLoan.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddFileDownloadInfoHandler : IRequestHandler<AddFileDownloadInfoCommand, ServiceResponse<FileDownloadInfoDto>>
    {
        private readonly IFileDownloadInfoRepository _FileDownloadInfoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddFileDownloadInfoHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public AddFileDownloadInfoHandler(
            IFileDownloadInfoRepository FileDownloadInfoRepository,
            IMapper mapper,
            ILogger<AddFileDownloadInfoHandler> logger,
            IUnitOfWork<LoanContext> uow,
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
