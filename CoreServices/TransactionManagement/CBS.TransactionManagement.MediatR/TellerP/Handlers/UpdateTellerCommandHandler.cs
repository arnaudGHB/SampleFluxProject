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

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on UpdateTellerCommand.
    /// </summary>
    public class UpdateTellerCommandHandler : IRequestHandler<UpdateTellerCommand, ServiceResponse<TellerDto>>
    {
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<UpdateTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _AccountRepository;
        private readonly ISavingProductRepository _SavingProductRepository;

        /// <summary>
        /// Constructor for initializing the UpdateTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateTellerCommandHandler(
            ITellerRepository TellerRepository,
            ILogger<UpdateTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IAccountRepository accountRepository = null,
            ISavingProductRepository savingProductRepository = null)
        {
            _TellerRepository = TellerRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _AccountRepository = accountRepository;
            _SavingProductRepository = savingProductRepository;
        }

        /// <summary>
        /// Handles the UpdateTellerCommand to update a Teller.
        /// </summary>
        /// <param name="request">The UpdateTellerCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TellerDto>> Handle(UpdateTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingTeller = await _TellerRepository.FindAsync(request.Id);
                if (existingTeller == null)
                {
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404);
                    return ServiceResponse<TellerDto>.Return404(errorMessage);
                }
                // Map the command to the entity
                _mapper.Map(request, existingTeller);

                // Retrieve primary teller account
                var primaryTellerAccount = await CheckTellerAccount(existingTeller);

                // Validate the request
                ValidateTellerRequest(request);

                var tellerWithSameNameOrCode = await _TellerRepository
                    .FindBy(t => (t.Name == request.Name || t.Code == request.Code) && t.Id != request.Id && t.BankId == existingTeller.BankId && t.BranchId == existingTeller.BranchId)
                    .FirstOrDefaultAsync();

                if (tellerWithSameNameOrCode != null)
                {
                    string errorMessage = $"Teller with name {request.Name} or code {request.Code} already exists in this bank and branch.";
                    _logger.LogError(errorMessage);
                    await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409);
                    return ServiceResponse<TellerDto>.Return409(errorMessage);
                }


                // Use the repository to update the existing Teller entity
                _TellerRepository.Update(existingTeller);
                await _uow.SaveAsync();

                // Log success and return response
                var response = ServiceResponse<TellerDto>.ReturnResultWith200(_mapper.Map<TellerDto>(existingTeller));
                _logger.LogInformation($"Teller {request.Name} was successfully updated.");
                await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, $"Teller {request.Id} was successfully updated.", LogLevelInfo.Information.ToString(), 200);
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
                // Handle other exceptions
                string errorMessage = $"Error occurred while updating Teller: {e.Message}";
                _logger.LogError(errorMessage);
                await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500);
                return ServiceResponse<TellerDto>.Return500(e);
            }
        }
        private async Task<Account> CheckTellerAccount(Teller teller)
        {
            var account = await _AccountRepository.FindBy(a => a.TellerId == teller.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                account = await CreateTellerAccount(teller);
            }
            else
            {
                account.AccountType = teller.TellerType;
                account.AccountName = teller.Name;
                account.CustomerName = teller.Name;
                _AccountRepository.Update(account);
            }
            return account;
        }
        private async Task<Account> CreateTellerAccount(Teller tellerEntity)
        {
            try
            {
                //find a saving product by its name property
                var savingProduct = await _SavingProductRepository.FindBy(c => c.IsUsedForTellerProvisioning).FirstOrDefaultAsync();

                if (savingProduct == null)
                {
                    var errorMessage = $"Sorry you need to create {DefaultProducts.DAILYOPERATIONS.ToString()} product to manage teller's accounts";
                    throw new InvalidOperationException(errorMessage);
                }
                var tellerAccount = new Account
                {
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    AccountNumber = BaseUtilities.GenerateUniqueNumber(),
                    CreatedDate = tellerEntity.CreatedDate,
                    TellerId = tellerEntity.Id,
                    ProductId = savingProduct.Id,
                    BranchId = tellerEntity.BranchId,
                    BankId = tellerEntity.BankId,
                    IsTellerAccount=true,
                    AccountType = tellerEntity.TellerType,
                    AccountName = tellerEntity.Name,
                    CustomerName = tellerEntity.Name,
                    BranchCode = _userInfoToken.BranchCode, 
                    Status = AccountStatus.Open.ToString()
                };

                _AccountRepository.Add(tellerAccount);
                await _uow.SaveAsync();
                return tellerAccount;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while creating Teller account: {e.Message}";
                throw new InvalidOperationException(errorMessage);
            }
        }
        private async Task LogAndUpdateAuditLog(string action, UpdateTellerCommand request, string message, string logLevel, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, action, request, message, logLevel, statusCode, _userInfoToken.Token);
        }

        private bool ValidateTellerRequest(UpdateTellerCommand request)
        {
            if (request.MinimumAmountToManage >= request.MaximumAmountToManage || request.MinimumAmountToManage < 0)
            {
                throw new ArgumentException("The minimum alert balance must be less than the maximum alert balance and cannot be negative.");
            }
            else if (request.MinimumDepositAmount >= request.MaximumDepositAmount || request.MinimumDepositAmount < 0)
            {
                throw new ArgumentException("The minimum deposit amount must be less than the maximum deposit amount and cannot be negative.");
            }
            else if (request.MinimumWithdrawalAmount >= request.MaximumWithdrawalAmount || request.MinimumWithdrawalAmount < 0)
            {
                throw new ArgumentException("The minimum withdrawal amount must be less than the maximum withdrawal amount and cannot be negative.");
            }
            else if (request.MinimumTransferAmount >= request.MaximumTransferAmount || request.MinimumTransferAmount < 0)
            {
                throw new ArgumentException("The minimum transfer amount must be less than the maximum transfer amount and cannot be negative.");
            }
            return true;
            // Add validation for other properties if needed
        }



    }

}
