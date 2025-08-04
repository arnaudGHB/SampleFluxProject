using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.MediatR.Accounting.Queries;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on MobileMoneyTellerConfigurationCommand.
    /// </summary>
    public class MobileMoneyTellerConfigurationCommandHandler : IRequestHandler<MobileMoneyTellerConfigurationCommand, ServiceResponse<TellerDto>>
    {
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<MobileMoneyTellerConfigurationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _AccountRepository;
        private readonly ISavingProductRepository _SavingProductRepository;
        private readonly IMediator _mediator;  // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the MobileMoneyTellerConfigurationCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public MobileMoneyTellerConfigurationCommandHandler(
            ITellerRepository TellerRepository,
            ILogger<MobileMoneyTellerConfigurationCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IAccountRepository accountRepository = null,
            ISavingProductRepository savingProductRepository = null,
            IMediator mediator = null)
        {
            _TellerRepository = TellerRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _AccountRepository = accountRepository;
            _SavingProductRepository = savingProductRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the MobileMoneyTellerConfigurationCommand to update a Teller.
        /// </summary>
        /// <param name="request">The MobileMoneyTellerConfigurationCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <summary>
        /// Handles the configuration of a Mobile Money Teller, including updating its account number,
        /// validating input data, and ensuring proper account type for non-member transactions.
        /// </summary>
        /// <param name="request">The MobileMoneyTellerConfigurationCommand containing the request data.</param>
        /// <param name="cancellationToken">A CancellationToken for managing task cancellation.</param>
        /// <returns>A ServiceResponse object containing the updated TellerDto or an error message.</returns>
        public async Task<ServiceResponse<TellerDto>> Handle(MobileMoneyTellerConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate and retrieve existing teller by ID
                var existingTeller = await _TellerRepository.FindAsync(request.Id);
                if (existingTeller == null)
                {
                    string errorMessage = $"Teller with ID {request.Id} was not found.";
                    _logger.LogError(errorMessage);
                    await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404);
                    return ServiceResponse<TellerDto>.Return404(errorMessage);
                }

                // Map request data to the existing teller entity
                _mapper.Map(request, existingTeller);

                // Retrieve the associated account for the teller
                var account = await _AccountRepository.RetrieveTellerAccount(existingTeller);
                if (account == null)
                {
                    throw new InvalidOperationException("Teller account not found.");
                }

                string accountNumber = request.AccountNumber;
                string newAccNumber = string.Empty;

                if (request.Option != null)
                {
                    // Retrieve the base account number from an external source
                    var getAccountNumber = new GetAccountNumberByIDCommand { Id = request.AccountNumber };
                    var resultGetAccountNumber = await _mediator.Send(getAccountNumber);

                    if (resultGetAccountNumber.StatusCode == 200)
                    {
                        // Validate and ensure account number base is exactly 6 digits
                        var accountNumberBase = resultGetAccountNumber.Data.AccountNumber;
                        if (string.IsNullOrEmpty(accountNumberBase))
                        {
                            throw new InvalidOperationException("Account number base cannot be null or empty.");
                        }

                        if (accountNumberBase.Length < 6)
                        {
                            accountNumberBase = accountNumberBase.PadRight(6, '0');
                        }
                        else if (accountNumberBase.Length > 6)
                        {
                            throw new InvalidOperationException("Account number base exceeds the required length of 6 characters.");
                        }

                        // Retrieve and validate the non-member account
                        var nonMemberAccount = await _AccountRepository.GetAccountByAccountNumber(request.MapMobileMoneyToNoneMemberMobileMoneyReference);
                        if (nonMemberAccount == null)
                        {
                            throw new InvalidOperationException("The provided non-member account number does not exist. Please verify and try again.");
                        }

                        // Ensure non-member account type is valid (either MobileMoneyMTN or MobileMoneyORANGE)
                        if (nonMemberAccount.AccountType != AccountType.MobileMoneyMTN.ToString() &&
                            nonMemberAccount.AccountType != AccountType.MobileMoneyORANGE.ToString())
                        {
                            throw new InvalidOperationException(
                                "The account type associated with the provided non-member account number is invalid. " +
                                "The account must be of type 'MobileMoneyMTN' or 'MobileMoneyORANGE'. Please verify and try again."
                            );
                        }

                        // Construct new account numbers for teller and non-member transactions
                        newAccNumber = $"{accountNumberBase}{nonMemberAccount.CustomerId}{existingTeller.Code}";
                        existingTeller.MapMobileMoneyToNoneMemberMobileMoneyReference = newAccNumber;
                        nonMemberAccount.AccountNumber = newAccNumber;

                        accountNumber = $"{accountNumberBase}{account.CustomerId}{existingTeller.Code}";
                        account.AccountNumber = accountNumber;

                        // Update the accounts
                        _AccountRepository.Update(account);
                        _AccountRepository.Update(nonMemberAccount);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Failed to retrieve account number. StatusCode: {resultGetAccountNumber.StatusCode}");
                    }
                }

                // Update the teller entity
                _TellerRepository.Update(existingTeller);
                await _uow.SaveAsync();

                // Log success and return the updated teller data
                var response = ServiceResponse<TellerDto>.ReturnResultWith200(_mapper.Map<TellerDto>(existingTeller));
                _logger.LogInformation($"Teller {existingTeller.Name} was successfully updated.");
                await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, $"Teller {existingTeller.Name} was successfully configured.", LogLevelInfo.Information.ToString(), 200);
                return response;
            }
            catch (ArgumentException e)
            {
                // Handle validation errors
                string errorMessage = $"Validation error: {e.Message}";
                _logger.LogError(errorMessage);
                await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 400);
                return ServiceResponse<TellerDto>.Return400(errorMessage);
            }
            catch (Exception e)
            {
                // Handle general errors
                string errorMessage = $"Error occurred while updating Teller: {e.Message}";
                _logger.LogError(errorMessage);
                await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500);
                return ServiceResponse<TellerDto>.Return500(e);
            }
        }

        private async Task LogAndUpdateAuditLog(string action, MobileMoneyTellerConfigurationCommand request, string message, string logLevel, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, action, request, message, logLevel, statusCode, _userInfoToken.Token);
        }

       



    }

}
