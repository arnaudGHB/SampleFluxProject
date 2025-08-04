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
    /// Handles the command to delete a DocumentName based on DeleteDocumentNameCommand.
    /// </summary>
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentRepository _DocumentNameRepository; // Repository for accessing DocumentName data.
        private readonly ILogger<DeleteDocumentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDocumentNameCommandHandler.
        /// </summary>
        /// <param name="DocumentNameRepository">Repository for DocumentName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDocumentCommandHandler(
            IDocumentRepository DocumentNameRepository, IMapper mapper,
            ILogger<DeleteDocumentCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _DocumentNameRepository = DocumentNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDocumentNameCommand to delete a DocumentName.
        /// </summary>
        /// <param name="request">The DeleteDocumentNameCommand containing DocumentName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DocumentName entity with the specified ID exists
                var existingDocumentName = await _DocumentNameRepository.FindAsync(request.Id);
                if (existingDocumentName == null)
                {
                    errorMessage = $"DocumentName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDocumentName.IsDeleted = true;

                _DocumentNameRepository.Update(existingDocumentName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting DocumentName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}