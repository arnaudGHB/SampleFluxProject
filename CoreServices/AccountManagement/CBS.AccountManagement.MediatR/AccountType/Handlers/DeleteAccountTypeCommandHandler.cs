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
    public class DeleteAccountTypeCommandHandler : IRequestHandler<DeleteAccountTypeCommand, ServiceResponse<bool>>
    {
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
        private readonly ILogger<DeleteAccountTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteAccountTypeCommandHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountTypeCommandHandler(
            IAccountTypeRepository AccountTypeRepository, IMapper mapper,
            ILogger<DeleteAccountTypeCommandHandler> logger
, IUnitOfWork<POSContext> uow,UserInfoToken userInfoToken
            )
        {
            _AccountTypeRepository = AccountTypeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteAccountTypeCommand to delete an AccountType.
        /// </summary>
        /// <param name="request">The DeleteAccountTypeCommand containing AccountType ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string errorMessage = null;

                var existingAccountType = GetExistingAccountType(request);

                if (existingAccountType == null)
                {
                    errorMessage = request.IdType == "ACCOUNTTYPE"
                        ? $"AccountType with ID {request.Id} not found."
                        : $"AccountType with Product ID {request.Id} not found.";

                    LogAndAuditError(request, errorMessage, LogLevelInfo.Information, request.IdType == "ACCOUNTTYPE" ? 409 : 404);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccountType.IsDeleted = true;
                _AccountTypeRepository.Update(existingAccountType);

                LogAuditSuccess(request);

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while deleting AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 500);
                return ServiceResponse<bool>.Return500(e);
            }
        }

        private AccountType GetExistingAccountType(DeleteAccountTypeCommand request)
        {
            return request.IdType switch
            {
                "ACCOUNTTYPE" => _AccountTypeRepository.FindAsync(request.Id).Result,
                "PRODUCT" => _AccountTypeRepository.FindBy(x => x.OperationAccountTypeId == request.Id).FirstOrDefault(),
                _ => null,
            };
        }

        private void LogAndAuditError(DeleteAccountTypeCommand request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteAccountTypeCommand",
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(DeleteAccountTypeCommand request)
        {
            string successMessage = "AccountType deleted successfully.";
            APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteAccountTypeCommand",
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}