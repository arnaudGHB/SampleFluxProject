using AutoMapper;
using CBS.TransactionManagement.MediatR.FeePolicyP.Commands;
using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FeePolicyP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteFeePolicyHandler : IRequestHandler<DeleteFeePolicyCommand, ServiceResponse<bool>>
    {
        private readonly IFeePolicyRepository _FeePolicyRepository; // Repository for accessing Fee data.
        private readonly ILogger<DeleteFeePolicyHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;

        /// <summary>
        /// Constructor for initializing the DeleteFeePolicyHandler.
        /// </summary>
        /// <param name="feeRepository">Repository for Fee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for database operations.</param>
        public DeleteFeePolicyHandler(
            IFeePolicyRepository feeRepository,
            IMapper mapper,
            ILogger<DeleteFeePolicyHandler> logger,
            IUnitOfWork<TransactionContext> unitOfWork)
        {
            _FeePolicyRepository = feeRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the DeleteFeeCommand to delete a Fee.
        /// </summary>
        /// <param name="request">The DeleteFeeCommand containing Fee ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteFeePolicyCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Fee entity with the specified ID exists
                var existingFee = await _FeePolicyRepository.FindAsync(request.Id);
                if (existingFee == null)
                {
                    errorMessage = $"Fee with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the Fee entity as deleted
                existingFee.IsDeleted = true;
                _FeePolicyRepository.Update(existingFee);

                // Save changes to the database
                await _unitOfWork.SaveAsync();

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
