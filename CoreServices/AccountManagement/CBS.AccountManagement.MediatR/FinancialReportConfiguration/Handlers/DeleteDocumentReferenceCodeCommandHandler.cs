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
    public class DeleteDocumentReferenceCodeCommandHandler : IRequestHandler<DeleteDocumentReferenceCodeCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICorrespondingMappingExceptionRepository _correspondingMappingExceptionRepository;
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        private readonly ILogger<DeleteDocumentReferenceCodeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ICashMovementTrackingConfigurationRepository? _chartOfAccountRepository;

        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public DeleteDocumentReferenceCodeCommandHandler(

            ILogger<DeleteDocumentReferenceCodeCommandHandler> logger,
            IMapper mapper,
            IDocumentReferenceCodeRepository? documentReferenceCodeRepository,
            IDocumentRepository? documentRepository,
                        ICashMovementTrackingConfigurationRepository? chartOfAccountRepository,
            ICorrespondingMappingExceptionRepository? correspondingMappingExceptionRepository,
            IDocumentTypeRepository? documentTypeRepository,
            ICorrespondingMappingRepository? correspondingMappingRepository,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _documentTypeRepository = documentTypeRepository;
            _documentRepository = documentRepository;
            _correspondingMappingExceptionRepository = correspondingMappingExceptionRepository;
            _correspondingMappingRepository = correspondingMappingRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentReferenceCodeCommand request, CancellationToken cancellationToken)
        {

            List<CorrespondingMapping> CorrespondingMappings = new List<CorrespondingMapping>();
            List<CorrespondingMappingException> CorrespondingMappingExceptions = new List<CorrespondingMappingException>();
            DocumentReferenceCode documentReferenceCode = new DocumentReferenceCode();
            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {

                var existingDocument = await _documentReferenceCodeRepository.FindAsync(request.Id);


                if (existingDocument == null)
                {
                    errorMessage = $"There is no document create with Id: {request.Id}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                CorrespondingMappings = _correspondingMappingRepository.FindBy(x => x.DocumentReferenceCodeId == request.Id).ToList();

                CorrespondingMappingExceptions = _correspondingMappingExceptionRepository.FindBy(x => x.DocumentReferenceCodeId == request.Id).ToList();

                if (CorrespondingMappingExceptions.Count()>0)
                {
                    _correspondingMappingExceptionRepository.RemoveRange(CorrespondingMappingExceptions);
                }
                if (CorrespondingMappings.Count() > 0)
                {
                    _correspondingMappingRepository.RemoveRange(CorrespondingMappings);
                }

        
                _documentReferenceCodeRepository.Remove(existingDocument);

                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true, $"Reference code:{documentReferenceCode.ReferenceCode} has been created successfully for report:{existingDocument.Document}");
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