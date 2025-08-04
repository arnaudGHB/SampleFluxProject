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
    /// Handles the command to delete a Account based on DeleteAccountCommand.
    /// </summary>
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<DeleteAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountCommandHandler(
         IAccountRepository AccountRepository, IMapper mapper,
         ILogger<DeleteAccountCommandHandler> logger
, IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }


        /// <summary>
        /// Handles the DeleteAccountCommand to delete a Account.
        /// </summary>
        /// <param name="request">The DeleteAccountCommand containing Account ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Account entity with the specified ID exists
                var existingAccount = await _AccountRepository.FindAsync(request.Id);
                if (existingAccount == null)
                {
                  
                    errorMessage = $"Account with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteAccountCommand",
                               JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                if (CheckIfTransactionExist(existingAccount))
                {
                    errorMessage = $"Account with ID {request.Id} cannot be deleted  not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteAccountCommand",
                             JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                existingAccount.IsDeleted = true;

                _AccountRepository.Update(existingAccount);
                await _uow.SaveAsync();
                errorMessage = $"Account you have successfully deleted account with ID {request.Id}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteAccountCommand",
                         request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";

                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteAccountCommand",
                 request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }

        private bool CheckIfTransactionExist(Data.Account Account)
        {
          return false;
        }
    }
}