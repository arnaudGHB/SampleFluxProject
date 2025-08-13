using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FeeMediaR.FeeP.Commands;
using CBS.TransactionManagement.Repository.FeeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FeeMediaR.FeeP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteFeeHandler : IRequestHandler<DeleteFeeCommand, ServiceResponse<bool>>
    {
        private readonly IFeeRepository _FeeRepository; // Repository for accessing Fee data.
        private readonly ILogger<DeleteFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteFeeCommandHandler.
        /// </summary>
        /// <param name="FeeRepository">Repository for Fee data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteFeeHandler(
            IFeeRepository FeeRepository, IMapper mapper,
            ILogger<DeleteFeeHandler> logger, IUnitOfWork<TransactionContext> uow)
        {
            _FeeRepository = FeeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteFeeCommand to delete a Fee.
        /// </summary>
        /// <param name="request">The DeleteFeeCommand containing Fee ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteFeeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Fee entity with the specified ID exists
                var existingFee = await _FeeRepository.FindAsync(request.Id);
                if (existingFee == null)
                {
                    errorMessage = $"Fee with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingFee.IsDeleted = true;
                _FeeRepository.Update(existingFee);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Fee: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
