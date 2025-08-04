using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteDocumentHandler : IRequestHandler<DeleteDocumentCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentRepository _DocumentRepository; // Repository for accessing Document data.
        private readonly ILogger<DeleteDocumentHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDocumentCommandHandler.
        /// </summary>
        /// <param name="DocumentRepository">Repository for Document data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDocumentHandler(
            IDocumentRepository DocumentRepository, IMapper mapper,
            ILogger<DeleteDocumentHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _DocumentRepository = DocumentRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDocumentCommand to delete a Document.
        /// </summary>
        /// <param name="request">The DeleteDocumentCommand containing Document ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Document entity with the specified ID exists
                var existingDocument = await _DocumentRepository.FindAsync(request.Id);
                if (existingDocument == null)
                {
                    errorMessage = $"Document with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDocument.IsDeleted = true;
                //existingDocument.DeletedBy = request.UserId;
                _DocumentRepository.Update(existingDocument);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Document: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
