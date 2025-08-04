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
    /// Handles the command to update an Asset Type based on UpdateAssetTypeCommand.
    /// </summary>
    public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, ServiceResponse<AssetTypeDto>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository; // Repository for accessing AssetType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateAssetTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateAssetTypeCommandHandler.
        /// </summary>
        /// <param name="assetTypeRepository">Repository for AssetType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateAssetTypeCommandHandler(
            IAssetTypeRepository assetTypeRepository,
            IMapper mapper,
            ILogger<UpdateAssetTypeCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetTypeRepository = assetTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAssetTypeCommand to update an Asset Type.
        /// </summary>
        /// <param name="request">The UpdateAssetTypeCommand containing updated AssetType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetTypeDto>> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the AssetType entity to be updated from the repository
                var existingAssetType = await _assetTypeRepository.FindAsync(request.Id);

                // Step 2: Check if the AssetType entity exists
                if (existingAssetType != null)
                {
                    // Step 3: Update AssetType entity properties with values from the request
                    existingAssetType.TypeName = request.TypeName;
                    existingAssetType.UsefulLifeYears = request.UsefulLifeYears;

                    // Step 4: Use the repository to update the existing AssetType entity
                    _assetTypeRepository.Update(existingAssetType);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"AssetType '{existingAssetType.TypeName}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var assetTypeDto = _mapper.Map<AssetTypeDto>(existingAssetType);

                    // Step 9: Return the updated AssetTypeDto with a 200 status code
                    return ServiceResponse<AssetTypeDto>.ReturnResultWith200(assetTypeDto);
                }
                else
                {
                    // Step 10: If the AssetType entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"AssetType with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<AssetTypeDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AssetType: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AssetTypeDto>.Return500(errorMessage);
            }
        }
    }
}
