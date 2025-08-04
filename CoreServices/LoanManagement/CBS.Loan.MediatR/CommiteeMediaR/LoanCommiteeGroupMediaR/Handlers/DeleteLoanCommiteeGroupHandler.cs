using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeGroupMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanCommiteeGroupHandler : IRequestHandler<DeleteLoanCommiteeGroupCommand, ServiceResponse<bool>>
    {
        private readonly ILoanCommiteeGroupRepository _LoanCommiteeGroupRepository; // Repository for accessing LoanCommiteeGroup data.
        private readonly ILogger<DeleteLoanCommiteeGroupHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanCommiteeGroupCommandHandler.
        /// </summary>
        /// <param name="LoanCommiteeGroupRepository">Repository for LoanCommiteeGroup data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanCommiteeGroupHandler(
            ILoanCommiteeGroupRepository LoanCommiteeGroupRepository, IMapper mapper,
            ILogger<DeleteLoanCommiteeGroupHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanCommiteeGroupRepository = LoanCommiteeGroupRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanCommiteeGroupCommand to delete a LoanCommiteeGroup.
        /// </summary>
        /// <param name="request">The DeleteLoanCommiteeGroupCommand containing LoanCommiteeGroup ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanCommiteeGroupCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanCommiteeGroup entity with the specified ID exists
                var existingLoanCommiteeGroup = await _LoanCommiteeGroupRepository.FindAsync(request.Id);
                if (existingLoanCommiteeGroup == null)
                {
                    errorMessage = $"LoanCommiteeGroup with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanCommiteeGroup.IsDeleted = true;
                _LoanCommiteeGroupRepository.Update(existingLoanCommiteeGroup);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanCommiteeGroup: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
