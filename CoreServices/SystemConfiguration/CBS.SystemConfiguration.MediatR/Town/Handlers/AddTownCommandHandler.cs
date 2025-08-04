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
    /// Handles the command to add a new TownType.
    /// </summary>
    public class AddTownCommandHandler : IRequestHandler<AddTownCommand, ServiceResponse<TownDto>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTownCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddTownCommandHandler(
            ITownRepository TownTypeRepository,
            IMapper mapper,
            ILogger<AddTownCommandHandler> logger,
            IUnitOfWork<SystemContext> uow, UserInfoToken userInfoToken)
        {
            _TownRepository = TownTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddTownCommand to add a new Town.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TownDto>> Handle(AddTownCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Town with the same name already exists (case-insensitive)
                var existingTownType = _TownRepository.All.Where(c => c.Name == request.Name);
                if (existingTownType.Any())
                {
                    var errorMessage = $"Town : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TownDto>.Return409(errorMessage);
                }
                  Town modelExType = _mapper.Map<Town>(request);
              
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DOC");
                _TownRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<TownDto>(modelExType);
                return ServiceResponse< TownDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Town: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse< TownDto>.Return500(e);
            }
        }
    }
}