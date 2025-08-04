using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanApplicationFeeHandler : IRequestHandler<DeleteLoanApplicationFeeCommand, ServiceResponse<bool>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanApplicationFee data.
        private readonly ILogger<DeleteLoanApplicationFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanApplicationFeeHandler.
        /// </summary>
        /// <param name="loanApplicationFeeRepository">Repository for LoanApplicationFee data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of Work for database operations.</param>
        public DeleteLoanApplicationFeeHandler(
            ILoanApplicationFeeRepository loanApplicationFeeRepository,
            ILogger<DeleteLoanApplicationFeeHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow)
        {
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanApplicationFeeCommand to delete a LoanApplicationFee.
        /// </summary>
        /// <param name="request">The DeleteLoanApplicationFeeCommand containing LoanApplicationFee ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanApplicationFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the LoanApplicationFee entity with the specified ID exists
                var existingLoanApplicationFee = await _loanApplicationFeeRepository.FindAsync(request.Id);
                if (existingLoanApplicationFee == null)
                {
                    var errorMessage = $"LoanApplicationFee with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanApplicationFee.IsDeleted = true;
                _loanApplicationFeeRepository.Update(existingLoanApplicationFee);
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while deleting LoanApplicationFee: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
