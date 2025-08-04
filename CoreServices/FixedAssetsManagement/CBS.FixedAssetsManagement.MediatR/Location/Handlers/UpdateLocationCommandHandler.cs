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
    /// Handles the command to update a Location based on UpdateLocationCommand.
    /// </summary>
    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, ServiceResponse<LocationDto>>
    {
        private readonly ILocationRepository _locationRepository; // Repository for accessing Location data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateLocationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateLocationCommandHandler.
        /// </summary>
        /// <param name="locationRepository">Repository for Location data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateLocationCommandHandler(
            ILocationRepository locationRepository,
            IMapper mapper,
            ILogger<UpdateLocationCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLocationCommand to update a Location.
        /// </summary>
        /// <param name="request">The UpdateLocationCommand containing updated location data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LocationDto>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Location entity to be updated from the repository
                var existingLocation = await _locationRepository.FindAsync( request.Id);

                // Step 2: Check if the Location entity exists
                if (existingLocation != null)
                {
                    // Step 3: Update Location entity properties with values from the request
                    existingLocation.LocationName = request.LocationName;
                    existingLocation.Address = request.Address;

                    // Step 4: Use the repository to update the existing Location entity
                    _locationRepository.Update(existingLocation);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Location '{existingLocation.LocationName}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var locationDto = _mapper.Map<LocationDto>(existingLocation);

                    // Step 9: Return the updated LocationDto with a 200 status code
                    return ServiceResponse<LocationDto>.ReturnResultWith200(locationDto);
                }
                else
                {
                    // Step 10: If the Location entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Location with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<LocationDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Location: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<LocationDto>.Return500(errorMessage);
            }
        }
    }
}
