using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Commands;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteFeeRangeHandler : IRequestHandler<DeleteFeeRangeCommand, ServiceResponse<bool>>
    {
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing Fee data.
        private readonly ILogger<DeleteFeeRangeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _unitOfWork;

        /// <summary>
        /// Constructor for initializing the DeleteFeeRangeHandler.
        /// </summary>
        /// <param name="feeRepository">Repository for Fee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for database operations.</param>
        public DeleteFeeRangeHandler(
            IFeeRangeRepository feeRepository,
            IMapper mapper,
            ILogger<DeleteFeeRangeHandler> logger,
            IUnitOfWork<LoanContext> unitOfWork)
        {
            _feeRangeRepository = feeRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the DeleteLoanTermCommand to delete a Fee.
        /// </summary>
        /// <param name="request">The DeleteLoanTermCommand containing Fee ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteFeeRangeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Fee entity with the specified ID exists
                var existingFee = await _feeRangeRepository.FindAsync(request.Id);
                if (existingFee == null)
                {
                    errorMessage = $"Fee with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the Fee entity as deleted
                existingFee.IsDeleted = true;
                _feeRangeRepository.Update(existingFee);

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
