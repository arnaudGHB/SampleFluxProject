using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using FluentValidation.Validators;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class UpdateCommandDocumentReferenceCodeHandler : IRequestHandler<UpdateCommandDocumentReferenceCode, ServiceResponse<bool>>
    {
            private readonly ILogger<UpdateCommandDocumentReferenceCodeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
 
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCommandDocumentReferenceCodeHandler(

            ILogger<UpdateCommandDocumentReferenceCodeHandler> logger,
            IMapper mapper,
            IDocumentReferenceCodeRepository? documentReferenceCodeRepository,

   
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
 
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
             _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
 
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateCommandDocumentReferenceCode request, CancellationToken cancellationToken)
        {

                    string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {
                var existingDocument = await _documentReferenceCodeRepository.FindAsync(request.Id);

                // Check if the Document entity exists
                if (existingDocument != null)
                {
                    // Update Document entity properties with values from the request
                    existingDocument.DocumentId = request.DocumentId;
                    existingDocument.DocumentTypeId = request.DocumentTypeId;
                    existingDocument = _mapper.Map(request,existingDocument);
                    // Use the repository to update the existing Document entity
                    _documentReferenceCodeRepository.Update(existingDocument);
                    await _uow.SaveAsync();
                    errorMessage = $"Document reference code {request.Id} was successfully updated.";
                    _logger.LogInformation(errorMessage);
                    return ServiceResponse<bool>.ReturnResultWith200(true,errorMessage);
                }
                else
                {
                    // If the Document entity was not found, return 404 Not Found response with an error message
                      errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while saving DocumentReferenceCOde: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddEntryTempDataCommand",
                  request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }

        
    }
}