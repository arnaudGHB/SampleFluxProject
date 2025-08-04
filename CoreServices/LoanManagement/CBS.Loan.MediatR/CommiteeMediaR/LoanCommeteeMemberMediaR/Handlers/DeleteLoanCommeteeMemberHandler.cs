using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanCommeteeMemberCommandHandler : IRequestHandler<DeleteLoanCommeteeMemberCommand, ServiceResponse<bool>>
    {
        private readonly ILoanCommeteeMemberRepository _LoanCommeteeMemberRepository; // Repository for accessing LoanCommeteeMember data.
        private readonly ILogger<DeleteLoanCommeteeMemberCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanCommeteeMemberCommandHandler.
        /// </summary>
        /// <param name="LoanCommeteeMemberRepository">Repository for LoanCommeteeMember data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanCommeteeMemberCommandHandler(
            ILoanCommeteeMemberRepository LoanCommeteeMemberRepository, IMapper mapper,
            ILogger<DeleteLoanCommeteeMemberCommandHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanCommeteeMemberRepository = LoanCommeteeMemberRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanCommeteeMemberCommand to delete a LoanCommeteeMember.
        /// </summary>
        /// <param name="request">The DeleteLoanCommeteeMemberCommand containing LoanCommeteeMember ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanCommeteeMemberCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanCommeteeMember entity with the specified ID exists
                var existingLoanCommeteeMember = await _LoanCommeteeMemberRepository.FindAsync(request.Id);
                if (existingLoanCommeteeMember == null)
                {
                    errorMessage = $"LoanCommeteeMember with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanCommeteeMember.IsDeleted = true;
                existingLoanCommeteeMember.DeletedBy = request.UserId;
                _LoanCommeteeMemberRepository.Update(existingLoanCommeteeMember);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanCommeteeMember: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
