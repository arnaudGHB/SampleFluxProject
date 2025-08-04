using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanProductRepaymentCycleCommandHandler : IRequestHandler<DeleteLoanProductRepaymentCycleCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductRepaymentCycleRepository _LoanProductRepaymentCycleRepository; // Repository for accessing LoanProductRepaymentCycle data.
        private readonly ILogger<DeleteLoanProductRepaymentCycleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanProductRepaymentCycleCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentCycleRepository">Repository for LoanProductRepaymentCycle data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanProductRepaymentCycleCommandHandler(
            ILoanProductRepaymentCycleRepository LoanProductRepaymentCycleRepository, IMapper mapper,
            ILogger<DeleteLoanProductRepaymentCycleCommandHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanProductRepaymentCycleRepository = LoanProductRepaymentCycleRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanProductRepaymentCycleCommand to delete a LoanProductRepaymentCycle.
        /// </summary>
        /// <param name="request">The DeleteLoanProductRepaymentCycleCommand containing LoanProductRepaymentCycle ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanProductRepaymentCycleCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanProductRepaymentCycle entity with the specified ID exists
                var existingLoanProductRepaymentCycle = await _LoanProductRepaymentCycleRepository.FindAsync(request.Id);
                if (existingLoanProductRepaymentCycle == null)
                {
                    errorMessage = $"LoanProductRepaymentCycle with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanProductRepaymentCycle.IsDeleted = true;
                _LoanProductRepaymentCycleRepository.Update(existingLoanProductRepaymentCycle);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanProductRepaymentCycle: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
