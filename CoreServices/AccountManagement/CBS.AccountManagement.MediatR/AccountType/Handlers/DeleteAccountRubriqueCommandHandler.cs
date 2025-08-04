using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a AccountType based on DeleteAccountTypeCommand.
    /// </summary>
//    public class DeleteAccountRubriqueCommandHandler : IRequestHandler<DeleteAccountRubriqueCommand, ServiceResponse<bool>>
//    {
//        private readonly IAccountRubriqueRepository _accountRubriqueRepository; // Repository for accessing AccountType data.
//        private readonly ILogger<DeleteAccountRubriqueCommandHandler> _logger; // Logger for logging handler actions and errors.
//        private readonly IMapper _mapper; // AutoMapper for object mapping.
//        private readonly IUnitOfWork<POSContext> _uow;
//        private readonly UserInfoToken _userInfoToken;

//        /// <summary>
//        /// Constructor for initializing the DeleteAccountTypeCommandHandler.
//        /// </summary>
//        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
//        /// <param name="logger">Logger for logging handler actions and errors.</param>
//        public DeleteAccountRubriqueCommandHandler(
//            IAccountRubriqueRepository accountRubriqueRepository, IMapper mapper,
//            ILogger<DeleteAccountRubriqueCommandHandler> logger
//, IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken
//            )
//        {
//            _accountRubriqueRepository = accountRubriqueRepository;
//            _logger = logger;
//            _mapper = mapper;
//            _uow = uow;
//            _userInfoToken = userInfoToken;
//        }

//        /// <summary>
//        /// Handles the DeleteAccountRubriqueCommand to delete an AccountRubrique.
//        /// </summary>
//        /// <param name="request">The DeleteAccountTypeCommand containing AccountType ID to be deleted.</param>
//        /// <param name="cancellationToken">A cancellation token.</param>
//        public async Task<ServiceResponse<bool>> Handle(DeleteAccountRubriqueCommand request, CancellationToken cancellationToken)
//        {
           

//                try
//                {
//                    List<AccountRubrique> accountRubriques = new List<AccountRubrique>();
//                      foreach (var item in request.ListOfOperationEventAttributeId)
//                        {
//                            var existingOperationAccounts = _accountRubriqueRepository.FindBy(f => f.OperationEventAttributeId.Equals(item));
//                            if (existingOperationAccounts != null)
//                            {
//                                AccountRubrique model = existingOperationAccounts.FirstOrDefault();
//                                model.IsDeleted = true;
//                                accountRubriques.Add(model);
//                            }

//                        }
//                    var result = _mapper.Map<AccountRubricResponseDto>(accountRubriques);
//                    return ServiceResponse<bool>.ReturnResultWith200(true);
//                }
//                catch (Exception e)
//                {
//                    // Log error and return 500 Internal Server Error response with an error message
//                    string errorMessage = $"Error occurred while updating AccountType: {e.Message}";
//                    LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 500);
//                    return ServiceResponse<bool>.Return500(e);
//                }

//            }

       

//        private bool GetAccountType(string operationAccountType)
//            {
//                return operationAccountType == "LOAN" || operationAccountType == "SAVING";
//            }



//            private void LogAndAuditError(DeleteAccountRubriqueCommand request, string errorMessage, LogLevelInfo logLevel, int statusCode)
//            {
//                _logger.LogError(errorMessage);
//                APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountRubriqueCommand",
//                    JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode, _userInfoToken.Token).Wait();
//            }

//            private void LogAuditSuccess(DeleteAccountRubriqueCommand request)
//            {
//                string successMessage = "AccountType update successfully.";
//                APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountRubriqueCommand",
//                    JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
//            }
       
//    }
}