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
    public class DeleteCorrespondingMappingExceptionCommandHandler : IRequestHandler<DeleteCorrespondingMappingExceptionCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;

        private readonly ICorrespondingMappingExceptionRepository _correspondingMappingExceptionRepository;
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        private readonly ILogger<DeleteCorrespondingMappingExceptionCommandHandler> _logger; // Logger for logging handler actions and errors.
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
        public DeleteCorrespondingMappingExceptionCommandHandler(

            ILogger<DeleteCorrespondingMappingExceptionCommandHandler> logger,
            IMapper mapper,

                        ICashMovementTrackingConfigurationRepository? chartOfAccountRepository,
            ICorrespondingMappingExceptionRepository? correspondingMappingExceptionRepository,
            IDocumentTypeRepository? documentTypeRepository,
            ICorrespondingMappingRepository? correspondingMappingRepository,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {

            _correspondingMappingExceptionRepository = correspondingMappingExceptionRepository;
            _correspondingMappingRepository = correspondingMappingRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteCorrespondingMappingExceptionCommand request, CancellationToken cancellationToken)
        {



            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {


                var model = await _correspondingMappingExceptionRepository.FindAsync(request.Id);


                if (model == null)
                {
                    errorMessage = $"There is no correspondingMapping  with Id: {request.Id}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(),
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                _correspondingMappingExceptionRepository.Remove(model);


                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true, $"Corresponding mapping has been successfully deleted:{model.Id}");
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