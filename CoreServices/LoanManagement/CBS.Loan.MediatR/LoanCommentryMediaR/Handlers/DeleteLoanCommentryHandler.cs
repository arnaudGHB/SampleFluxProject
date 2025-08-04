using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCommentryMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanCommentryHandler : IRequestHandler<DeleteLoanCommentryCommand, ServiceResponse<bool>>
    {
        private readonly ILoanCommentryRepository _LoanCommentryRepository; // Repository for accessing LoanCommentry data.
        private readonly ILogger<DeleteLoanCommentryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanCommentryCommandHandler.
        /// </summary>
        /// <param name="LoanCommentryRepository">Repository for LoanCommentry data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanCommentryHandler(
            ILoanCommentryRepository LoanCommentryRepository, IMapper mapper,
            ILogger<DeleteLoanCommentryHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanCommentryRepository = LoanCommentryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanCommentryCommand to delete a LoanCommentry.
        /// </summary>
        /// <param name="request">The DeleteLoanCommentryCommand containing LoanCommentry ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanCommentryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanCommentry entity with the specified ID exists
                var existingLoanCommentry = await _LoanCommentryRepository.FindAsync(request.Id);
                if (existingLoanCommentry == null)
                {
                    errorMessage = $"LoanCommentry with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanCommentry.IsDeleted = true;
                _LoanCommentryRepository.Update(existingLoanCommentry);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanCommentry: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
