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
    public class DeleteCommandCorrespondingMappingHandler : IRequestHandler<DeleteCommandCorrespondingMapping, ServiceResponse<bool>>
    {
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
           private readonly ILogger<DeleteCommandCorrespondingMappingHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
     
        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public DeleteCommandCorrespondingMappingHandler(

            ILogger<DeleteCommandCorrespondingMappingHandler> logger,
            IMapper mapper,
            ICorrespondingMappingRepository? documentReferenceCodeRepository,
                      IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _correspondingMappingRepository = documentReferenceCodeRepository;
        ;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
          
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteCommandCorrespondingMapping request, CancellationToken cancellationToken)
        {

                        string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {

                var existingDocument = await _correspondingMappingRepository.FindAsync(request.Id);




                _correspondingMappingRepository.Remove(existingDocument);

                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true, $"Corresponding mapping:{existingDocument.AccountNumber} has been deleted successfully");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while saving Corresponding mapping: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(),
                  request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}