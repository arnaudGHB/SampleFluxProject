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
    public class AddDocumentCommandHandler : IRequestHandler<AddDocumentCommand, ServiceResponse<DocumentDto>>
    {
        private readonly IDocumentRepository _documentRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDocumentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddDocumentCommandHandler(
            IDocumentRepository documentTypeRepository,
            IMapper mapper,
            ILogger<AddDocumentCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _documentRepository = documentTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddDocumentCommand to add a new DocumentType.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentDto>> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a DocumentType with the same name already exists (case-insensitive)
                var existingDocumentType = _documentRepository.All.Where(c => c.Name == request.Name);
                if (existingDocumentType.Any())
                {
                    var errorMessage = $"Document : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentDto>.Return409(errorMessage);
                }
                Data.Document modelExType = _mapper.Map<Data.Document>(request);
                // Add the new OperationEventName entity to the repository
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DOC");
                _documentRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<Data.DocumentDto>(modelExType);
                return ServiceResponse<Data.DocumentDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Document: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<Data.DocumentDto>.Return500(e);
            }
        }
    }
}