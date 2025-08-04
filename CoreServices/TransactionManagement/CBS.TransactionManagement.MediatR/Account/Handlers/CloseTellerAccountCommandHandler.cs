using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper.Helper;
using Microsoft.EntityFrameworkCore;
using System;

namespace CBS.AccountManagement.Handlers
{
    /// <summary>
    /// Handles the command to update an Account based on UpdateAccountCommand.
    /// </summary>
    public class CloseTellerAccountCommandHandler : IRequestHandler<CloseTellerAccountCommand, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<CloseTellerAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public CloseTellerAccountCommandHandler(
            IAccountRepository AccountRepository,
            ILogger<CloseTellerAccountCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountCommand to update an Account.
        /// </summary>
        /// <param name="request">The UpdateAccountCommand containing updated Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(CloseTellerAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Account.Status = AccountStatus.Closed.ToString();
                // Use the repository to update the existing Account entity
                _AccountRepository.Update(request.Account);

                // Save changes using Unit of Work
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, "An error updating the accounts status", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return500();
                }

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<AccountDto>.ReturnResultWith200(_mapper.Map<AccountDto>(request.Account));
                _logger.LogInformation($"Teller Account was successfully closed.");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, $"Teller Account was successfully closed.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return response;

            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }
    }
}
