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
    /// Handles the command to add a new DivisionType.
    /// </summary>
    public class AddDivisionCommandHandler : IRequestHandler<AddDivisionCommand, ServiceResponse<DivisionDto>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddDivisionCommandHandler(
            IDivisionRepository DivisionTypeRepository,
            IMapper mapper,
            ILogger<AddDivisionCommandHandler> logger,
            IUnitOfWork<SystemContext> uow, UserInfoToken userInfoToken)
        {
            _DivisionRepository = DivisionTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddDivisionCommand to add a new Division.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DivisionDto>> Handle(AddDivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Division with the same name already exists (case-insensitive)
                var existingDivisionType = _DivisionRepository.All.Where(c => c.Name == request.Name);
                if (existingDivisionType.Any())
                {
                    var errorMessage = $"Division : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DivisionDto>.Return409(errorMessage);
                }
                  Division modelExType = _mapper.Map<Division>(request);
              
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DOC");
                _DivisionRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<DivisionDto>(modelExType);
                return ServiceResponse< DivisionDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Division: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse< DivisionDto>.Return500(e);
            }
        }
    }
}