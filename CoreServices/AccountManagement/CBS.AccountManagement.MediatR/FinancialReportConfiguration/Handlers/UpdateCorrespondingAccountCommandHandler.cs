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
    public class UpdateCorrespondingAccountCommandHandler : IRequestHandler<UpdateCorrespondingAccountCommand, ServiceResponse<bool>>
    {
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        private readonly ILogger<UpdateCorrespondingAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IChartOfAccountRepository? _chartOfAccountRepository;
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCorrespondingAccountCommandHandler(

            ILogger<UpdateCorrespondingAccountCommandHandler> logger,
            IMapper mapper,
            IDocumentReferenceCodeRepository? documentReferenceCodeRepository,

                        IChartOfAccountRepository? chartOfAccountRepository,
   
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, ICorrespondingMappingRepository? correspondingMappingRepository)
        {
            _correspondingMappingRepository = correspondingMappingRepository;
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
             _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateCorrespondingAccountCommand request, CancellationToken cancellationToken)
        {

            CorrespondingMapping correspondingMapping = new CorrespondingMapping();
            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {

                correspondingMapping = await _correspondingMappingRepository.FindAsync(request.Id);
                if (correspondingMapping == null)
                {
                    errorMessage = $"There is no correspondingMapping  with DocumentRefenceCodeIdId: {request.DocumentRefenceCodeId}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(),
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
               var documentreference= await _documentReferenceCodeRepository.FindAsync(request.DocumentRefenceCodeId);
                if (documentreference == null)
                {
                    errorMessage = $"There is no document reference  with DocumentRefenceCodeId: {request.DocumentRefenceCodeId}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(),
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                var account = await _chartOfAccountRepository.FindAsync(request.ChartOfAccountId);
                if (documentreference == null)
                {
                    errorMessage = $"There is no account  with Id: {request.ChartOfAccountId}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(),
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
   
                correspondingMapping.AccountNumber = account.AccountNumber;
        

                _correspondingMappingRepository.Update(correspondingMapping);
  
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true,$"Reference code:{documentreference.ReferenceCode} has been update successfully for report:{documentreference.Document}");
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