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
    /// Handles the command to update a Depreciation Entry based on UpdateDepreciationEntryCommand.
    /// </summary>
    public class UpdateDepreciationEntryCommandHandler : IRequestHandler<UpdateDepreciationEntryCommand, ServiceResponse<DepreciationEntryDto>>
    {
        private readonly IDepreciationEntryRepository _depreciationEntryRepository; // Repository for accessing Depreciation Entry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateDepreciationEntryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateDepreciationEntryCommandHandler.
        /// </summary>
        /// <param name="depreciationEntryRepository">Repository for Depreciation Entry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateDepreciationEntryCommandHandler(
            IDepreciationEntryRepository depreciationEntryRepository,
            IMapper mapper,
            ILogger<UpdateDepreciationEntryCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _depreciationEntryRepository = depreciationEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDepreciationEntryCommand to update a Depreciation Entry.
        /// </summary>
        /// <param name="request">The UpdateDepreciationEntryCommand containing updated depreciation entry data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DepreciationEntryDto>> Handle(UpdateDepreciationEntryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Depreciation Entry entity to be updated from the repository
                var existingDepreciationEntry = await _depreciationEntryRepository.FindAsync(request.Id);

                // Step 2: Check if the Depreciation Entry entity exists
                if (existingDepreciationEntry != null)
                {
                    // Step 3: Update Depreciation Entry entity properties with values from the request
                    existingDepreciationEntry.AssetId = request.AssetId;
                    //existingDepreciationEntry.AssetName = request.AssetName;
                    existingDepreciationEntry.DepreciationDate = request.DepreciationDate;
                    existingDepreciationEntry.DepreciationAmount = request.DepreciationAmount;
                    existingDepreciationEntry.BookValueAfter = request.BookValueAfter;

                    // Step 4: Use the repository to update the existing Depreciation Entry entity
                    _depreciationEntryRepository.Update(existingDepreciationEntry);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Depreciation Entry for Asset '{request.AssetName}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var depreciationEntryDto = _mapper.Map<DepreciationEntryDto>(existingDepreciationEntry);

                    // Step 9: Return the updated DepreciationEntryDto with a 200 status code
                    return ServiceResponse<DepreciationEntryDto>.ReturnResultWith200(depreciationEntryDto);
                }
                else
                {
                    // Step 10: If the Depreciation Entry entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Depreciation Entry with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<DepreciationEntryDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Depreciation Entry: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<DepreciationEntryDto>.Return500(errorMessage);
            }
        }
    }
}
