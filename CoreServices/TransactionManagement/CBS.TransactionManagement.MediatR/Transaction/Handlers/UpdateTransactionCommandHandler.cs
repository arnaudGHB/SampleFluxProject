using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a Transaction based on UpdateTransactionCommand.
    /// </summary>
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, ServiceResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<UpdateTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateTransactionCommandHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateTransactionCommandHandler(
            ITransactionRepository TransactionRepository,
            ILogger<UpdateTransactionCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _TransactionRepository = TransactionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateTransactionCommand to update a Transaction.
        /// </summary>
        /// <param name="request">The UpdateTransactionCommand containing updated Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransactionDto>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Transaction entity to be updated from the repository
                var existingTransaction = await _TransactionRepository.FindAsync(request.Id);

                // Check if the Transaction entity exists
                if (existingTransaction != null)
                {
                    // Update Transaction entity properties with values from the request
                    existingTransaction.Amount = request.Amount;
                    existingTransaction.TransactionType = request.TransactionType;
                    existingTransaction.TransactionReference = request.TransactionRef;
                    // Use the repository to update the existing Transaction entity
                    _TransactionRepository.Update(existingTransaction);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<TransactionDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<TransactionDto>.ReturnResultWith200(_mapper.Map<TransactionDto>(existingTransaction));
                    _logger.LogInformation($"Transaction {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Transaction entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }
    }

}
