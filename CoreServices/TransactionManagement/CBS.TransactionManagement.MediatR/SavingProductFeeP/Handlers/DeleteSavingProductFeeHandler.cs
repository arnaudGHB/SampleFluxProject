using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands;
using CBS.TransactionManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteSavingProductFeeHandler : IRequestHandler<DeleteSavingProductFeeCommand, ServiceResponse<bool>>
    {
        private readonly ISavingProductFeeRepository _SavingProductFeeRepository; // Repository for accessing SavingProductFee data.
        private readonly ILogger<DeleteSavingProductFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteSavingProductFeeHandler.
        /// </summary>
        /// <param name="SavingProductFeeRepository">Repository for SavingProductFee data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of Work for database operations.</param>
        public DeleteSavingProductFeeHandler(
            ISavingProductFeeRepository SavingProductFeeRepository,
            ILogger<DeleteSavingProductFeeHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow)
        {
            _SavingProductFeeRepository = SavingProductFeeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteSavingProductFeeCommand to delete a SavingProductFee.
        /// </summary>
        /// <param name="request">The DeleteSavingProductFeeCommand containing SavingProductFee ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteSavingProductFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the SavingProductFee entity with the specified ID exists
                var existingSavingProductFee = await _SavingProductFeeRepository.FindAsync(request.Id);
                if (existingSavingProductFee == null)
                {
                    var errorMessage = $"SavingProductFee with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingSavingProductFee.IsDeleted = true;
                _SavingProductFeeRepository.Update(existingSavingProductFee);
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while deleting SavingProductFee: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
