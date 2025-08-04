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
    /// Handles the command to delete a OperationEventNameAttributes based on DeleteOperationEventNameAttributesCommand.
    /// </summary>
    public class DeleteOperationEventAttributesCommandHandler : IRequestHandler<DeleteOperationEventAttributesCommand, ServiceResponse<bool>>
    {
        private readonly IOperationEventAttributeRepository _OperationEventNameAttributesRepository; // Repository for accessing OperationEventNameAttributes data.
        private readonly ILogger<DeleteOperationEventAttributesCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteOperationEventNameAttributesCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameAttributesRepository">Repository for OperationEventNameAttributes data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOperationEventAttributesCommandHandler(
            IOperationEventAttributeRepository OperationEventNameAttributesRepository, IMapper mapper,
            ILogger<DeleteOperationEventAttributesCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _OperationEventNameAttributesRepository = OperationEventNameAttributesRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOperationEventNameAttributesCommand to delete a OperationEventNameAttributes.
        /// </summary>
        /// <param name="request">The DeleteOperationEventNameAttributesCommand containing OperationEventNameAttributes ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOperationEventAttributesCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OperationEventNameAttributes entity with the specified ID exists
                var existingOperationEventNameAttributes = await _OperationEventNameAttributesRepository.FindAsync(request.Id);
                if (existingOperationEventNameAttributes == null)
                {
                    errorMessage = $"OperationEventNameAttributes with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingOperationEventNameAttributes.IsDeleted = true;

                _OperationEventNameAttributesRepository.Update(existingOperationEventNameAttributes);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OperationEventNameAttributes: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}