using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Commands;
using CBS.NLoan.Repository.FileDownloadInfoP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteFileDownloadInfoHandler : IRequestHandler<DeleteFileDownloadInfoCommand, ServiceResponse<bool>>
    {
        private readonly IFileDownloadInfoRepository _FileDownloadInfoRepository; // Repository for accessing FileDownloadInfo data.
        private readonly ILogger<DeleteFileDownloadInfoHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteFileDownloadInfoCommandHandler.
        /// </summary>
        /// <param name="FileDownloadInfoRepository">Repository for FileDownloadInfo data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteFileDownloadInfoHandler(
            IFileDownloadInfoRepository FileDownloadInfoRepository, IMapper mapper,
            ILogger<DeleteFileDownloadInfoHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _FileDownloadInfoRepository = FileDownloadInfoRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteFileDownloadInfoCommand to delete a FileDownloadInfo.
        /// </summary>
        /// <param name="request">The DeleteFileDownloadInfoCommand containing FileDownloadInfo ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteFileDownloadInfoCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the FileDownloadInfo entity with the specified ID exists
                var existingFileDownloadInfo = await _FileDownloadInfoRepository.FindAsync(request.Id);
                if (existingFileDownloadInfo == null)
                {
                    errorMessage = $"FileDownloadInfo with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                string msg = $"File not found at path: {existingFileDownloadInfo.FullPath}";
                // Delete the file from the file system
                if (!string.IsNullOrEmpty(existingFileDownloadInfo.FullPath))
                {
                    if (File.Exists(existingFileDownloadInfo.FullPath))
                    {
                        File.Delete(existingFileDownloadInfo.FullPath);
                        msg = string.Empty;
                    }
                    else
                    {
                        _logger.LogWarning(msg);
                    }
                }

                //Mark the entity as deleted
                existingFileDownloadInfo.IsDeleted = true;
                _FileDownloadInfoRepository.Update(existingFileDownloadInfo);

                // Save changes to the database
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true, $"Deletion of file {existingFileDownloadInfo.FileName} was successful. {msg}");
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting FileDownloadInfo: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
