using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OperationEventNameAttributes based on UpdateOperationEventNameAttributesCommand.
    /// </summary>
    public class UpdateOperationEventAttributesCommandHandler : IRequestHandler<UpdateOperationEventAttributesCommand, ServiceResponse<OperationEventAttributesDto>>
    {
        private readonly IOperationEventAttributeRepository _OperationEventNameAttributesRepository; // Repository for accessing OperationEventNameAttributes data.
        private readonly ILogger<UpdateOperationEventAttributesCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        //private readonly IOperationEventRepository _OperationEventRepository;
        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameAttributesCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameAttributesRepository">Repository for OperationEventNameAttributes data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOperationEventAttributesCommandHandler(
            IOperationEventAttributeRepository OperationEventNameAttributesRepository,
            ILogger<UpdateOperationEventAttributesCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            //_OperationEventRepository = OperationEventRepository;
            _OperationEventNameAttributesRepository = OperationEventNameAttributesRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOperationEventNameAttributesCommand to update a OperationEventNameAttributes.
        /// </summary>
        /// <param name="request">The UpdateOperationEventNameAttributesCommand containing updated OperationEventNameAttributes data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OperationEventAttributesDto>> Handle(UpdateOperationEventAttributesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the OperationEventNameAttributes entity to be updated from the repository
                var existingOperationEventNameAttributes = await _OperationEventNameAttributesRepository.FindAsync(request.Id);

                // Check if the OperationEventNameAttributes entity exists
                if (existingOperationEventNameAttributes != null)
                {
                    // Update OperationEventNameAttributes entity properties with values from the request
                    existingOperationEventNameAttributes.Name = request.Name;
                    //existingOperationEventNameAttributes.OperationEventId = request.OperationEventId;

                    // Use the repository to update the existing OperationEventNameAttributes entity
                    _OperationEventNameAttributesRepository.Update(existingOperationEventNameAttributes);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<OperationEventAttributesDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<OperationEventAttributesDto>.ReturnResultWith200(_mapper.Map<OperationEventAttributesDto>(existingOperationEventNameAttributes));
                    _logger.LogInformation($"OperationEventNameAttributes {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the OperationEventNameAttributes entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OperationEventAttributesDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating OperationEventNameAttributes: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OperationEventAttributesDto>.Return500(e);
            }
        }
    }
}