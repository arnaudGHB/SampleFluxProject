using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to delete a TransferLimits based on DeleteTransferLimitsCommand.
    /// </summary>
    public class DeleteTransferLimitsCommandHandler : IRequestHandler<DeleteTransferLimitsCommand, ServiceResponse<bool>>
    {
        private readonly ITransferLimitsRepository _TransferLimitsRepository; // Repository for accessing TransferLimits data.
        private readonly ILogger<DeleteTransferLimitsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteTransferLimitsCommandHandler.
        /// </summary>
        /// <param name="TransferLimitsRepository">Repository for TransferLimits data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteTransferLimitsCommandHandler(
            ITransferLimitsRepository TransferLimitsRepository, IMapper mapper,
            ILogger<DeleteTransferLimitsCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _TransferLimitsRepository = TransferLimitsRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteTransferLimitsCommand to delete a TransferLimits.
        /// </summary>
        /// <param name="request">The DeleteTransferLimitsCommand containing TransferLimits ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTransferLimitsCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the TransferLimits entity with the specified ID exists
                var existingTransferLimits = await _TransferLimitsRepository.FindAsync(request.Id);
                if (existingTransferLimits == null)
                {
                    errorMessage = $"TransferLimits with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingTransferLimits.IsDeleted = true;
                _TransferLimitsRepository.Update(existingTransferLimits);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting TransferLimits: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
