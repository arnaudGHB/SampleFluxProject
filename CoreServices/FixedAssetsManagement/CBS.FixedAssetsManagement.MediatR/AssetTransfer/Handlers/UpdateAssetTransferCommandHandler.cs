using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;
using System.Threading;
using System.Threading.Tasks;
using CBS.FixedAssetsManagement.MediatR.Commands;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update an Asset Transfer based on UpdateAssetTransferCommand.
    /// </summary>
    public class UpdateAssetTransferCommandHandler : IRequestHandler<UpdateAssetTransferCommand, ServiceResponse<AssetTransferDto>>
    {
        private readonly IAssetTransferRepository _assetTransferRepository; // Repository for accessing Asset Transfer data.
        private readonly ILogger<UpdateAssetTransferCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        /// <summary>
        /// Constructor for initializing the UpdateAssetTransferCommandHandler.
        /// </summary>
        /// <param name="assetTransferRepository">Repository for Asset Transfer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public UpdateAssetTransferCommandHandler(
            IAssetTransferRepository assetTransferRepository,
            ILogger<UpdateAssetTransferCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken,
            IMapper? mapper)
        {
            _assetTransferRepository = assetTransferRepository;
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateAssetTransferCommand to update an Asset Transfer.
        /// </summary>
        /// <param name="request">The UpdateAssetTransferCommand containing the updated Asset Transfer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetTransferDto>> Handle(UpdateAssetTransferCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Asset Transfer entity with the specified ID exists
                var existingTransfer = await _assetTransferRepository.FindAsync(request.Id);
                if (existingTransfer == null)
                {
                    errorMessage = $"Asset Transfer with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTransferDto>.Return404(errorMessage);
                }

                // Step 2: Update the properties of the existing asset transfer
                existingTransfer.TransferDate = request.TransferDate;
                existingTransfer.Reason = request.Reason;

                // Step 3: Update the asset transfer in the repository
                _assetTransferRepository.Update(existingTransfer);

                // Step 4: Save changes in the unit of work
                await _uow.SaveAsync();
                var AssetTransferDto = _mapper.Map<AssetTransferDto>(existingTransfer);
                // Step 5: Return a successful response
                return ServiceResponse<AssetTransferDto>.ReturnResultWith200(AssetTransferDto);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while updating Asset Transfer: {e.Message}";

                // Step 6: Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetTransferDto>.Return500(e);
            }
        }
    }
}
