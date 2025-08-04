using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteDocumentPackCommandHandler : IRequestHandler<DeleteDocumentPackCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentPackRepository _DocumentPackRepository; // Repository for accessing DocumentPack data.
        private readonly ILogger<DeleteDocumentPackCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDocumentPackCommandHandler.
        /// </summary>
        /// <param name="DocumentPackRepository">Repository for DocumentPack data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDocumentPackCommandHandler(
            IDocumentPackRepository DocumentPackRepository, IMapper mapper,
            ILogger<DeleteDocumentPackCommandHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _DocumentPackRepository = DocumentPackRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDocumentPackCommand to delete a DocumentPack.
        /// </summary>
        /// <param name="request">The DeleteDocumentPackCommand containing DocumentPack ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentPackCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DocumentPack entity with the specified ID exists
                var existingDocumentPack = await _DocumentPackRepository.FindAsync(request.Id);
                if (existingDocumentPack == null)
                {
                    errorMessage = $"DocumentPack with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDocumentPack.IsDeleted = true;
                existingDocumentPack.DeletedBy = request.UserId;
                _DocumentPackRepository.Update(existingDocumentPack);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting DocumentPack: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
