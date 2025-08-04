 
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Repository;
using AutoMapper;
using CBS.FixedAssetsManagement.MediatR.Commands;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Location based on AddLocationCommand.
    /// </summary>
    public class AddLocationCommandHandler : IRequestHandler<AddLocationCommand, ServiceResponse<LocationDto>>
    {
        private readonly ILocationRepository _locationRepository; // Repository for accessing Location data.
        private readonly ILogger<AddLocationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        // Constructor for initializing the AddLocationCommandHandler.
        public AddLocationCommandHandler(
            ILocationRepository locationRepository,
            ILogger<AddLocationCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _locationRepository = locationRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        // Handles the AddLocationCommand to add a new location.
        public async Task<ServiceResponse<LocationDto>> Handle(AddLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a location with the same name already exists.
                var existingLocation = await _locationRepository.FindBy(x => x.LocationName == request.LocationName).FirstOrDefaultAsync();
                string message = $"Location '{request.LocationName}' created successfully.";

                if (existingLocation != null)
                {
                    message = $"Location '{request.LocationName}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<LocationDto>.Return409(message);
                }

                // Map the request to a Location entity.
                var location = _mapper.Map<Location>(request);
                location.Id = BaseUtilities.GenerateInsuranceUniqueNumber(25,"LCT"); // Let the database generate the ID

                // Add the new location to the repository.
                _locationRepository.Add(location);
                await _uow.SaveAsync();

                // Log successful creation of the location.
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the location entity back to a DTO for response.
                var locationDto = _mapper.Map<LocationDto>(location);
                return ServiceResponse<LocationDto>.ReturnResultWith200(locationDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding location: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<LocationDto>.Return500(msg);
            }
        }
    }
}
