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
    /// Handles the command to update a Depreciation Method based on UpdateDepreciationMethodCommand.
    /// </summary>
    public class UpdateDepreciationMethodCommandHandler : IRequestHandler<UpdateDepreciationMethodCommand, ServiceResponse<DepreciationMethodDto>>
    {
        private readonly IDepreciationMethodRepository _depreciationMethodRepository; // Repository for accessing Depreciation Method data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateDepreciationMethodCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateDepreciationMethodCommandHandler.
        /// </summary>
        /// <param name="depreciationMethodRepository">Repository for Depreciation Method data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateDepreciationMethodCommandHandler(
            IDepreciationMethodRepository depreciationMethodRepository,
            IMapper mapper,
            ILogger<UpdateDepreciationMethodCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _depreciationMethodRepository = depreciationMethodRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDepreciationMethodCommand to update a Depreciation Method.
        /// </summary>
        /// <param name="request">The UpdateDepreciationMethodCommand containing updated depreciation method data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DepreciationMethodDto>> Handle(UpdateDepreciationMethodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Depreciation Method entity to be updated from the repository
                var existingMethod = await _depreciationMethodRepository.FindAsync(request.Id);

                // Step 2: Check if the Depreciation Method entity exists
                if (existingMethod != null)
                {
                    // Step 3: Update Depreciation Method entity properties with values from the request
                    existingMethod.MethodName = request.MethodName;
                    existingMethod.Description = request.Description;

                    // Step 4: Use the repository to update the existing Depreciation Method entity
                    _depreciationMethodRepository.Update(existingMethod);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync( );

                    // Step 6: Log success message
                    string msg = $"Depreciation Method '{existingMethod.MethodName}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var methodDto = _mapper.Map<DepreciationMethodDto>(existingMethod);

                    // Step 9: Return the updated DepreciationMethodDto with a 200 status code
                    return ServiceResponse<DepreciationMethodDto>.ReturnResultWith200(methodDto);
                }
                else
                {
                    // Step 10: If the Depreciation Method entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Depreciation Method with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<DepreciationMethodDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Depreciation Method: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<DepreciationMethodDto>.Return500(errorMessage);
            }
        }
    }
}
