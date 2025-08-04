using AutoMapper;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Repository;
using MediatR;
 
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
  
    /// <summary>
    /// Handles the command to add a new RegionType.
    /// </summary>
    public class AddRegionCommandHandler : IRequestHandler<AddRegionCommand, ServiceResponse<RegionDto>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddRegionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddRegionCommandHandler(
            IRegionRepository RegionTypeRepository,
            IMapper mapper,
            ILogger<AddRegionCommandHandler> logger,
            IUnitOfWork<SystemContext> uow, UserInfoToken userInfoToken)
        {
            _RegionRepository = RegionTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddRegionCommand to add a new Region.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RegionDto>> Handle(AddRegionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Region with the same name already exists (case-insensitive)
                var existingRegionType = _RegionRepository.All.Where(c => c.Name == request.Name);
                if (existingRegionType.Any())
                {
                    var errorMessage = $"Region : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RegionDto>.Return409(errorMessage);
                }
                  Region modelExType = _mapper.Map<Region>(request);
              
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DOC");
                _RegionRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<RegionDto>(modelExType);
                return ServiceResponse< RegionDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Region: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse< RegionDto>.Return500(e);
            }
        }
    }
}