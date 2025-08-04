using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Commands;
using CBS.NLoan.Repository.WriteOffLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteWriteOffLoanHandler : IRequestHandler<DeleteWriteOffLoanCommand, ServiceResponse<bool>>
    {
        private readonly IWriteOffLoanRepository _WriteOffLoanRepository; // Repository for accessing WriteOffLoan data.
        private readonly ILogger<DeleteWriteOffLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteWriteOffLoanCommandHandler.
        /// </summary>
        /// <param name="WriteOffLoanRepository">Repository for WriteOffLoan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteWriteOffLoanHandler(
            IWriteOffLoanRepository WriteOffLoanRepository, IMapper mapper,
            ILogger<DeleteWriteOffLoanHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _WriteOffLoanRepository = WriteOffLoanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteWriteOffLoanCommand to delete a WriteOffLoan.
        /// </summary>
        /// <param name="request">The DeleteWriteOffLoanCommand containing WriteOffLoan ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteWriteOffLoanCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the WriteOffLoan entity with the specified ID exists
                var existingWriteOffLoan = await _WriteOffLoanRepository.FindAsync(request.Id);
                if (existingWriteOffLoan == null)
                {
                    errorMessage = $"WriteOffLoan with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingWriteOffLoan.IsDeleted = true;
                _WriteOffLoanRepository.Update(existingWriteOffLoan);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting WriteOffLoan: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
