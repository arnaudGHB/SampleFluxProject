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
    /// Handles the command to delete a WithdrawalLimits based on DeleteWithdrawalLimitsCommand.
    /// </summary>
    public class DeleteWithdrawalLimitsCommandHandler : IRequestHandler<DeleteWithdrawalLimitsCommand, ServiceResponse<bool>>
    {
        private readonly IWithdrawalLimitsRepository _WithdrawalLimitsRepository; // Repository for accessing WithdrawalLimits data.
        private readonly ILogger<DeleteWithdrawalLimitsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteWithdrawalLimitsCommandHandler.
        /// </summary>
        /// <param name="WithdrawalLimitsRepository">Repository for WithdrawalLimits data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteWithdrawalLimitsCommandHandler(
            IWithdrawalLimitsRepository WithdrawalLimitsRepository, IMapper mapper,
            ILogger<DeleteWithdrawalLimitsCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _WithdrawalLimitsRepository = WithdrawalLimitsRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteWithdrawalLimitsCommand to delete a WithdrawalLimits.
        /// </summary>
        /// <param name="request">The DeleteWithdrawalLimitsCommand containing WithdrawalLimits ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteWithdrawalLimitsCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the WithdrawalLimits entity with the specified ID exists
                var existingWithdrawalLimits = await _WithdrawalLimitsRepository.FindAsync(request.Id);
                if (existingWithdrawalLimits == null)
                {
                    errorMessage = $"WithdrawalLimits with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingWithdrawalLimits.IsDeleted = true;
                _WithdrawalLimitsRepository.Update(existingWithdrawalLimits);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting WithdrawalLimits: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
