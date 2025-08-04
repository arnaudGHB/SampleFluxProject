using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a DocumentType based on UpdateDocumentTypeCommand.
    /// </summary>
    public class UpdateDocumentTypeCommandHandler : IRequestHandler<UpdateDocumentTypeCommand, ServiceResponse<DocumentTypeDto>>
    {
        private readonly IDocumentTypeRepository _documentTypeRepository; // Repository for accessing DocumentType data.
        private readonly ILogger<UpdateDocumentTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDocumentTypeCommandHandler.
        /// </summary>
        /// <param name="DocumentTypeRepository">Repository for DocumentType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDocumentTypeCommandHandler(
            IDocumentTypeRepository documentTypeRepository,
            ILogger<UpdateDocumentTypeCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _documentTypeRepository = documentTypeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDocumentTypeCommand to update a DocumentType.
        /// </summary>
        /// <param name="request">The UpdateDocumentTypeCommand containing updated DocumentType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentTypeDto>> Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the DocumentType entity to be updated from the repository
                var existingDocumentType = await _documentTypeRepository.FindAsync(request.Id);

                // Check if the DocumentType entity exists
                if (existingDocumentType != null)
                {
                    // Update DocumentType entity properties with values from the request
                    existingDocumentType.Name= request.Name;
                    existingDocumentType.DocumentId= request.DocumentId;
                    existingDocumentType.Description= request.Description;
                    existingDocumentType = _mapper.Map(request, existingDocumentType);
                    // Use the repository to update the existing DocumentType entity
                    _documentTypeRepository.Update(existingDocumentType);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<DocumentTypeDto>.ReturnResultWith200(_mapper.Map<DocumentTypeDto>(existingDocumentType));
                    _logger.LogInformation($"DocumentType {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the DocumentType entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentTypeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating DocumentType: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentTypeDto>.Return500(e);
            }
        }
    }
}