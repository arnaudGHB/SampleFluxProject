using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteDocumentAttachedToLoanHandler : IRequestHandler<DeleteDocumentAttachedToLoanCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentAttachedToLoanRepository _DocumentAttachedToLoanRepository; // Repository for accessing DocumentAttachedToLoan data.
        private readonly ILogger<DeleteDocumentAttachedToLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDocumentAttachedToLoanCommandHandler.
        /// </summary>
        /// <param name="DocumentAttachedToLoanRepository">Repository for DocumentAttachedToLoan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDocumentAttachedToLoanHandler(
            IDocumentAttachedToLoanRepository DocumentAttachedToLoanRepository, IMapper mapper,
            ILogger<DeleteDocumentAttachedToLoanHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _DocumentAttachedToLoanRepository = DocumentAttachedToLoanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDocumentAttachedToLoanCommand to delete a DocumentAttachedToLoan.
        /// </summary>
        /// <param name="request">The DeleteDocumentAttachedToLoanCommand containing DocumentAttachedToLoan ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentAttachedToLoanCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DocumentAttachedToLoan entity with the specified ID exists
                var existingDocumentAttachedToLoan = await _DocumentAttachedToLoanRepository.FindAsync(request.Id);
                if (existingDocumentAttachedToLoan == null)
                {
                    errorMessage = $"DocumentAttachedToLoan with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDocumentAttachedToLoan.IsDeleted = true;
                _DocumentAttachedToLoanRepository.Update(existingDocumentAttachedToLoan);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting DocumentAttachedToLoan: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
