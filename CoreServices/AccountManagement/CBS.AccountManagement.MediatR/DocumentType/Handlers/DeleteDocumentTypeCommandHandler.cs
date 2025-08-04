using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a DocumentTypeName based on DeleteDocumentTypeNameCommand.
    /// </summary>
    public class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentTypeRepository _DocumentTypeNameRepository; // Repository for accessing DocumentTypeName data.
        private readonly ILogger<DeleteDocumentTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDocumentTypeNameCommandHandler.
        /// </summary>
        /// <param name="DocumentTypeNameRepository">Repository for DocumentTypeName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDocumentTypeCommandHandler(
            IDocumentTypeRepository DocumentTypeNameRepository, IMapper mapper,
            ILogger<DeleteDocumentTypeCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _DocumentTypeNameRepository = DocumentTypeNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDocumentTypeNameCommand to delete a DocumentTypeName.
        /// </summary>
        /// <param name="request">The DeleteDocumentTypeNameCommand containing DocumentTypeName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DocumentTypeName entity with the specified ID exists
                var existingDocumentTypeName = await _DocumentTypeNameRepository.FindAsync(request.Id);
                if (existingDocumentTypeName == null)
                {
                    errorMessage = $"DocumentTypeName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDocumentTypeName.IsDeleted = true;

                _DocumentTypeNameRepository.Update(existingDocumentTypeName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting DocumentTypeName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}