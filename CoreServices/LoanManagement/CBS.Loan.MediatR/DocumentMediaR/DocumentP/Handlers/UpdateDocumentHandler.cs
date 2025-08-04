using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, ServiceResponse<DocumentDto>>
    {
        private readonly IDocumentRepository _DocumentRepository; // Repository for accessing Document data.
        private readonly ILogger<UpdateDocumentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDocumentCommandHandler.
        /// </summary>
        /// <param name="DocumentRepository">Repository for Document data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDocumentCommandHandler(
            IDocumentRepository DocumentRepository,
            ILogger<UpdateDocumentCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _DocumentRepository = DocumentRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDocumentCommand to update a Document.
        /// </summary>
        /// <param name="request">The UpdateDocumentCommand containing updated Document data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentDto>> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Document entity to be updated from the repository
                var existingDocument = await _DocumentRepository.FindAsync(request.Id);

                // Check if the Document entity exists
                if (existingDocument != null)
                {
                    // Update Document entity properties with values from the request
                    _mapper.Map(request, existingDocument);
                    // Use the repository to update the existing Document entity
                    _DocumentRepository.Update(existingDocument);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<DocumentDto>.ReturnResultWith200(_mapper.Map<DocumentDto>(existingDocument));
                    _logger.LogInformation($"Document {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Document entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Document: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentDto>.Return500(e);
            }
        }
    }

}
