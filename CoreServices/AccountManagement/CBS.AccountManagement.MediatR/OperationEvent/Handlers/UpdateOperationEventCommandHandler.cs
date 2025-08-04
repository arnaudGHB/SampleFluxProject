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
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class UpdateOperationEventCommandHandler : IRequestHandler<UpdateOperationEventCommand, ServiceResponse<OperationEventDto>>
    {
        private readonly IOperationEventRepository _OperationEventNameRepository; // Repository for accessing OperationEventName data.
        private readonly ILogger<UpdateOperationEventCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOperationEventCommandHandler(
            IOperationEventRepository OperationEventNameRepository,
            ILogger<UpdateOperationEventCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _OperationEventNameRepository = OperationEventNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOperationEventNameCommand to update a OperationEventName.
        /// </summary>
        /// <param name="request">The UpdateOperationEventNameCommand containing updated OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OperationEventDto>> Handle(UpdateOperationEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the OperationEventName entity to be updated from the repository
                var existingOperationEventName = await _OperationEventNameRepository.FindAsync(request.Id);

                // Check if the OperationEventName entity exists
                if (existingOperationEventName != null)
                {
                    // Update OperationEventName entity properties with values from the request
                    existingOperationEventName.OperationEventName= request.OperationEventName;
                    existingOperationEventName.EventCode= request.EventCode;
                    existingOperationEventName.Description= request.Description;
                    existingOperationEventName = _mapper.Map(request, existingOperationEventName);
                    // Use the repository to update the existing OperationEventName entity
                    _OperationEventNameRepository.Update(existingOperationEventName);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<OperationEventDto>.ReturnResultWith200(_mapper.Map<OperationEventDto>(existingOperationEventName));
                    _logger.LogInformation($"OperationEventName {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the OperationEventName entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OperationEventDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating OperationEventName: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OperationEventDto>.Return500(e);
            }
        }
    }
}