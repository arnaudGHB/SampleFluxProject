using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RescheduleLoanMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.RescheduleLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteRescheduleLoanHandler : IRequestHandler<DeleteRescheduleLoanCommand, ServiceResponse<bool>>
    {
        private readonly IRescheduleLoanRepository _RescheduleLoanRepository; // Repository for accessing RescheduleLoan data.
        private readonly ILogger<DeleteRescheduleLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteRescheduleLoanCommandHandler.
        /// </summary>
        /// <param name="RescheduleLoanRepository">Repository for RescheduleLoan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteRescheduleLoanHandler(
            IRescheduleLoanRepository RescheduleLoanRepository, IMapper mapper,
            ILogger<DeleteRescheduleLoanHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _RescheduleLoanRepository = RescheduleLoanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteRescheduleLoanCommand to delete a RescheduleLoan.
        /// </summary>
        /// <param name="request">The DeleteRescheduleLoanCommand containing RescheduleLoan ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteRescheduleLoanCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the RescheduleLoan entity with the specified ID exists
                var existingRescheduleLoan = await _RescheduleLoanRepository.FindAsync(request.Id);
                if (existingRescheduleLoan == null)
                {
                    errorMessage = $"RescheduleLoan with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingRescheduleLoan.IsDeleted = true;
                _RescheduleLoanRepository.Update(existingRescheduleLoan);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting RescheduleLoan: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
