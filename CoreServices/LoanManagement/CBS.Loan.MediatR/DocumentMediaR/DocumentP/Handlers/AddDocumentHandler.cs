using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddDocumentHandler : IRequestHandler<AddDocumentCommand, ServiceResponse<DocumentDto>>
    {
        private readonly IDocumentRepository _DocumentRepository; // Repository for accessing Document data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDocumentHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddDocumentCommandHandler.
        /// </summary>
        /// <param name="DocumentRepository">Repository for Document data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDocumentHandler(
            IDocumentRepository DocumentRepository,
            IMapper mapper,
            ILogger<AddDocumentHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _DocumentRepository = DocumentRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddDocumentCommand to add a new Document.
        /// </summary>
        /// <param name="request">The AddDocumentCommand containing Document data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentDto>> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Document with the same name already exists (case-insensitive)
                var existingDocument = await _DocumentRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Document with the same name already exists, return a conflict response
                if (existingDocument != null)
                {
                    var errorMessage = $"Document {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentDto>.Return409(errorMessage);
                }


                // Map the AddDocumentCommand to a Document entity
                var DocumentEntity = _mapper.Map<Document>(request);

                // Generate Id
                DocumentEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Convert UTC to local time and set it in the entity
                DocumentEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);

                // Add the new Document entity to the repository
                _DocumentRepository.Add(DocumentEntity);
                await _uow.SaveAsync();

                // Map the Document entity to DocumentDto and return it with a success response
                var DocumentDto = _mapper.Map<DocumentDto>(DocumentEntity);
                return ServiceResponse<DocumentDto>.ReturnResultWith200(DocumentDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Document: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentDto>.Return500(e);
            }
        }
    }

}
