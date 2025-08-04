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
    /// Handles the command to delete a Transaction based on DeleteTransactionCommand.
    /// </summary>
    public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, ServiceResponse<bool>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<DeleteTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteTransactionCommandHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteTransactionCommandHandler(
            ITransactionRepository TransactionRepository, IMapper mapper,
            ILogger<DeleteTransactionCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _TransactionRepository = TransactionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteTransactionCommand to delete a Transaction.
        /// </summary>
        /// <param name="request">The DeleteTransactionCommand containing Transaction ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Transaction entity with the specified ID exists
                var existingTransaction = await _TransactionRepository.FindAsync(request.Id);
                if (existingTransaction == null)
                {
                    errorMessage = $"Transaction with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingTransaction.IsDeleted = true;
                _TransactionRepository.Update(existingTransaction);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Transaction: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
