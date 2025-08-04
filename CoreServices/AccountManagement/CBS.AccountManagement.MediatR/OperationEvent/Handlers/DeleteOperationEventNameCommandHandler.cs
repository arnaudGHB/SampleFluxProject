using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a OperationEventName based on DeleteOperationEventNameCommand.
    /// </summary>
    public class DeleteOperationEventCommandHandler : IRequestHandler<DeleteOperationEventCommand, ServiceResponse<bool>>
    {
        private readonly IOperationEventRepository _OperationEventNameRepository; // Repository for accessing OperationEventName data.
        private readonly ILogger<DeleteOperationEventCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOperationEventCommandHandler(
            IOperationEventRepository OperationEventNameRepository, IMapper mapper,
            ILogger<DeleteOperationEventCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _OperationEventNameRepository = OperationEventNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOperationEventNameCommand to delete a OperationEventName.
        /// </summary>
        /// <param name="request">The DeleteOperationEventNameCommand containing OperationEventName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOperationEventCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OperationEventName entity with the specified ID exists
                var existingOperationEventName = await _OperationEventNameRepository.FindAsync(request.Id);
                if (existingOperationEventName == null)
                {
                    errorMessage = $"OperationEventName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingOperationEventName.IsDeleted = true;

                _OperationEventNameRepository.Update(existingOperationEventName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OperationEventName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}