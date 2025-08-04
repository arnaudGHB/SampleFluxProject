using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanAmortizationHandler : IRequestHandler<DeleteLoanAmortizationCommand, ServiceResponse<bool>>
    {
        private readonly ILoanAmortizationRepository _LoanAmortizationRepository; // Repository for accessing LoanAmortization data.
        private readonly ILogger<DeleteLoanAmortizationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanAmortizationCommandHandler.
        /// </summary>
        /// <param name="LoanAmortizationRepository">Repository for LoanAmortization data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanAmortizationHandler(
            ILoanAmortizationRepository LoanAmortizationRepository, IMapper mapper,
            ILogger<DeleteLoanAmortizationHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanAmortizationRepository = LoanAmortizationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanAmortizationCommand to delete a LoanAmortization.
        /// </summary>
        /// <param name="request">The DeleteLoanAmortizationCommand containing LoanAmortization ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanAmortizationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanAmortization entity with the specified ID exists
                var existingLoanAmortization = await _LoanAmortizationRepository.FindAsync(request.Id);
                if (existingLoanAmortization == null)
                {
                    errorMessage = $"LoanAmortization with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanAmortization.IsDeleted = true;
                _LoanAmortizationRepository.Update(existingLoanAmortization);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanAmortization: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
