using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeValidationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanCommiteeValidationHistoryHandler : IRequestHandler<DeleteLoanCommiteeValidationHistoryCommand, ServiceResponse<bool>>
    {
        private readonly ILoanCommiteeValidationHistoryRepository _LoanCommiteeValidationRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly ILogger<DeleteLoanCommiteeValidationHistoryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanCommiteeValidationCommandHandler.
        /// </summary>
        /// <param name="LoanCommiteeValidationRepository">Repository for LoanCommiteeValidation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanCommiteeValidationHistoryHandler(
            ILoanCommiteeValidationHistoryRepository LoanCommiteeValidationRepository, IMapper mapper,
            ILogger<DeleteLoanCommiteeValidationHistoryHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanCommiteeValidationRepository = LoanCommiteeValidationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanCommiteeValidationCommand to delete a LoanCommiteeValidation.
        /// </summary>
        /// <param name="request">The DeleteLoanCommiteeValidationCommand containing LoanCommiteeValidation ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanCommiteeValidationHistoryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanCommiteeValidation entity with the specified ID exists
                var existingLoanCommiteeValidation = await _LoanCommiteeValidationRepository.FindAsync(request.Id);
                if (existingLoanCommiteeValidation == null)
                {
                    errorMessage = $"LoanCommiteeValidation with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanCommiteeValidation.IsDeleted = true;
                _LoanCommiteeValidationRepository.Update(existingLoanCommiteeValidation);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanCommiteeValidation: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
