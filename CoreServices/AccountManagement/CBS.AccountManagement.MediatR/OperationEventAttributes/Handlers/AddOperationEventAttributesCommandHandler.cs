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
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class AddOperationEventAttributesCommandHandler : IRequestHandler<AddOperationEventAttributesCommand, ServiceResponse<OperationEventAttributesDto>>
    {
        private readonly IOperationEventAttributeRepository _OperationEventAttributesRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOperationEventAttributesCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IOperationEventRepository _OperationEventRepository; // Repository for accessing AccountCategory data.

        /// <summary>
        /// Constructor for initializing the AddOperationEventAttributesCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for OperationEventAttributes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOperationEventAttributesCommandHandler(
            IOperationEventAttributeRepository OperationEventAttributesRepository,
             IOperationEventRepository operationEventRepository,
            IMapper mapper,
            ILogger<AddOperationEventAttributesCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _OperationEventAttributesRepository = OperationEventAttributesRepository;
            _mapper = mapper;
            _logger = logger;
            _OperationEventRepository = operationEventRepository;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddOperationEventAttributesCommand to add a new OperationEventAttributes.
        /// </summary>
        /// <param name="request">The AddOperationEventAttributesCommand containing OperationEventAttributes data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OperationEventAttributesDto>> Handle(AddOperationEventAttributesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AccountCategory with the same name already exists (case-insensitive)
                var existingAccountCategory = await _OperationEventAttributesRepository.FindBy(c => c.Name == request.Name &&  c.OperationEventId==request.OperationEventId).FirstOrDefaultAsync();

                // If a AccountCategory with the same name already exists, return a conflict response
                if (existingAccountCategory != null)
                {
                    var errorMessage = $"OperationEventAttributes: {request.Name} has already been created for eventId:{request.OperationEventId} exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OperationEventAttributesDto>.Return409(errorMessage);
                }

              
                var OperationEventAttributesEntity = _mapper.Map<Data.OperationEventAttributes>(request);

                OperationEventAttributesEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12,"OEA");
                // Add the new OperationEventAttributes entity to the repository
                OperationEventAttributesEntity.OperationEventAttributeCode = (await _OperationEventRepository.FindAsync(request.OperationEventId)).EventCode;
                    _OperationEventAttributesRepository.Add(OperationEventAttributesEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<OperationEventAttributesDto>.Return500();
                }
                // Map the OperationEventAttributes entity to OperationEventAttributesDto and return it with a success response
                var OperationEventAttributesDto = _mapper.Map<OperationEventAttributesDto>(OperationEventAttributesEntity);
                return ServiceResponse<OperationEventAttributesDto>.ReturnResultWith200(OperationEventAttributesDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving OperationEventAttributes: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OperationEventAttributesDto>.Return500(e);
            }
        }
    }
}