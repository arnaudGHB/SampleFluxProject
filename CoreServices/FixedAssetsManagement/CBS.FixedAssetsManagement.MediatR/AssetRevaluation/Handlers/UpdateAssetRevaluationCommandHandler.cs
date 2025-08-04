using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.MediatR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update an Asset Revaluation based on UpdateAssetRevaluationCommand.
    /// </summary>
    public class UpdateAssetRevaluationCommandHandler : IRequestHandler<UpdateAssetRevaluationCommand, ServiceResponse<AssetRevaluationDto>>
    {
        private readonly IAssetRevaluationRepository _assetRevaluationRepository; // Repository for accessing Asset Revaluation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateAssetRevaluationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateAssetRevaluationCommandHandler.
        /// </summary>
        /// <param name="assetRevaluationRepository">Repository for Asset Revaluation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateAssetRevaluationCommandHandler(
            IAssetRevaluationRepository assetRevaluationRepository,
            IMapper mapper,
            ILogger<UpdateAssetRevaluationCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRevaluationRepository = assetRevaluationRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAssetRevaluationCommand to update an Asset Revaluation.
        /// </summary>
        /// <param name="request">The UpdateAssetRevaluationCommand containing updated revaluation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetRevaluationDto>> Handle(UpdateAssetRevaluationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Asset Revaluation entity to be updated from the repository
                var existingRevaluation = await _assetRevaluationRepository.FindAsync( request.Id);

                // Step 2: Check if the Asset Revaluation entity exists
                if (existingRevaluation != null)
                {
                    // Step 3: Update Asset Revaluation entity properties with values from the request
                    existingRevaluation.AssetId = request.AssetId;
                    existingRevaluation.RevaluationDate = request.RevaluationDate;
                    existingRevaluation.OldValue = request.OldValue;
                    existingRevaluation.NewValue = request.NewValue;
                    existingRevaluation.Reason = request.Reason;

                    // Step 4: Use the repository to update the existing Asset Revaluation entity
                    _assetRevaluationRepository.Update(existingRevaluation);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Asset Revaluation for AssetId '{existingRevaluation.AssetId}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var revaluationDto = _mapper.Map<AssetRevaluationDto>(existingRevaluation);

                    // Step 9: Return the updated AssetRevaluationDto with a 200 status code
                    return ServiceResponse<AssetRevaluationDto>.ReturnResultWith200(revaluationDto);
                }
                else
                {
                    // Step 10: If the Asset Revaluation entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Asset Revaluation with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<AssetRevaluationDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Asset Revaluation: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AssetRevaluationDto>.Return500(errorMessage);
            }
        }
    }
}

