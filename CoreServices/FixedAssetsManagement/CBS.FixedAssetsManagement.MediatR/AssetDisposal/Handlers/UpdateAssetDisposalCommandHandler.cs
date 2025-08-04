using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.FixedAssetsManagement.MediatR.Commands;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update an AssetDisposal based on UpdateAssetDisposalCommand.
    /// </summary>
    public class UpdateAssetDisposalCommandHandler : IRequestHandler<UpdateAssetDisposalCommand, ServiceResponse<AssetDisposalDto>>
    {
        private readonly IAssetDisposalRepository _assetDisposalRepository; // Repository for accessing AssetDisposal data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateAssetDisposalCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateAssetDisposalCommandHandler.
        /// </summary>
        /// <param name="assetDisposalRepository">Repository for AssetDisposal data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateAssetDisposalCommandHandler(
            IAssetDisposalRepository assetDisposalRepository,
            IMapper mapper,
            ILogger<UpdateAssetDisposalCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetDisposalRepository = assetDisposalRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAssetDisposalCommand to update an AssetDisposal.
        /// </summary>
        /// <param name="request">The UpdateAssetDisposalCommand containing updated asset disposal data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetDisposalDto>> Handle(UpdateAssetDisposalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the AssetDisposal entity to be updated from the repository
                var existingDisposal = await _assetDisposalRepository.FindAsync(request.Id);

                // Step 2: Check if the AssetDisposal entity exists
                if (existingDisposal != null)
                {
                    // Step 3: Update AssetDisposal entity properties with values from the request
                    existingDisposal.DisposalDate = request.DisposalDate;
                    existingDisposal.DisposalMethod = request.DisposalMethod;
                    existingDisposal.DisposalValue = request.DisposalValue;
                    existingDisposal.Reason = request.Reason;

                    // Step 4: Use the repository to update the existing AssetDisposal entity
                    _assetDisposalRepository.Update(existingDisposal);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Asset disposal '{existingDisposal.Id}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var disposalDto = _mapper.Map<AssetDisposalDto>(existingDisposal);

                    // Step 9: Return the updated AssetDisposalDto with a 200 status code
                    return ServiceResponse<AssetDisposalDto>.ReturnResultWith200(disposalDto);
                }
                else
                {
                    // Step 10: If the AssetDisposal entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Asset disposal with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<AssetDisposalDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Asset Disposal: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AssetDisposalDto>.Return500(errorMessage);
            }
        }
    }
}
