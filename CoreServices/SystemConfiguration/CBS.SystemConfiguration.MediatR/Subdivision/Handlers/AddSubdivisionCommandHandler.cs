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
    /// Handles the command to add a new SubdivisionType.
    /// </summary>
    public class AddSubdivisionCommandHandler : IRequestHandler<AddSubdivisionCommand, ServiceResponse<SubdivisionDto>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddSubdivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddSubdivisionCommandHandler(
            ISubdivisionRepository SubdivisionTypeRepository,
            IMapper mapper,
            ILogger<AddSubdivisionCommandHandler> logger,
            IUnitOfWork<SystemContext> uow, UserInfoToken userInfoToken)
        {
            _SubdivisionRepository = SubdivisionTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddSubdivisionCommand to add a new Subdivision.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubdivisionDto>> Handle(AddSubdivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Subdivision with the same name already exists (case-insensitive)
                var existingSubdivisionType = _SubdivisionRepository.All.Where(c => c.Name == request.Name);
                if (existingSubdivisionType.Any())
                {
                    var errorMessage = $"Subdivision : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<SubdivisionDto>.Return409(errorMessage);
                }
                  Subdivision modelExType = _mapper.Map<Subdivision>(request);
              
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DOC");
                _SubdivisionRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<SubdivisionDto>(modelExType);
                return ServiceResponse< SubdivisionDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Subdivision: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse< SubdivisionDto>.Return500(e);
            }
        }
    }
}