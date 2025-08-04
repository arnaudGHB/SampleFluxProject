using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new OperationEventName.
    /// </summary>
    public class AddOperationEventCommandHandler : IRequestHandler<AddOperationEventCommand, ServiceResponse<OperationEventDto>>
    {
        private readonly IOperationEventRepository _OperationEventNameRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOperationEventCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOperationEventCommandHandler(
            IOperationEventRepository OperationEventNameRepository,
            IMapper mapper,
            ILogger<AddOperationEventCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _OperationEventNameRepository = OperationEventNameRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddOperationEventNameCommand to add a new OperationEventName.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OperationEventDto>> Handle(AddOperationEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a OperationEventName with the same name already exists (case-insensitive)
                var existingOperationEventName = _OperationEventNameRepository.All.Where(c => c.EventCode == request.EventCode);

                // If a OperationEventName with the same name already exists, return a conflict response
                if (existingOperationEventName.Any())
                {
                    var errorMessage = $"OperationEventName {request.OperationEventName} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OperationEventDto>.Return409(errorMessage);
                }

                // Map the AddOperationEventNameAttributesCommand to a OperationEventNameAttributes entity
                var OperationEventNameAttributesEntity = _mapper.Map<OperationEvent>(request);

                OperationEventNameAttributesEntity = OperationEvent.SetOperationEventEntity(OperationEventNameAttributesEntity, _userInfoToken);
                // Add the new OperationEventName entity to the repository
                OperationEventNameAttributesEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "OE");
                _OperationEventNameRepository.Add(OperationEventNameAttributesEntity);
                await _uow.SaveAsync();

                // Map the OperationEventName entity to OperationEventNameDto and return it with a success response
                var OperationEventNameDto = _mapper.Map<OperationEventDto>(OperationEventNameAttributesEntity);
                return ServiceResponse<OperationEventDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving OperationEventName: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OperationEventDto>.Return500(e);
            }
        }
    }
}