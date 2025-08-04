using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
  
    /// <summary>
    /// Handles the command to add a new DocumentType.
    /// </summary>
    public class AddDocumentTypeCommandHandler : IRequestHandler<AddDocumentTypeCommand, ServiceResponse<DocumentTypeDto>>
    {
        private readonly IDocumentTypeRepository _documentTypeRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDocumentTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddDocumentTypeCommandHandler(
            IDocumentTypeRepository documentTypeRepository,
            IMapper mapper,
            ILogger<AddDocumentTypeCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _documentTypeRepository = documentTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddDocumentTypeCommand to add a new DocumentType.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentTypeDto>> Handle(AddDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a DocumentType with the same name already exists (case-insensitive)
                var existingDocumentType = _documentTypeRepository.All.Where(c => c.Name == request.Name && c.DocumentId == request.DocumentId);
                if (existingDocumentType.Any())
                {
                    var errorMessage = $"Document : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentTypeDto>.Return409(errorMessage);
                }
                Data.DocumentType modelExType = _mapper.Map<Data.DocumentType>(request);
                // Add the new OperationEventName entity to the repository
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "OE");
                _documentTypeRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<Data.DocumentTypeDto>(modelExType);
                return ServiceResponse<Data.DocumentTypeDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving OperationEventName: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<Data.DocumentTypeDto>.Return500(e);
            }
        }
    }
}