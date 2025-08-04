using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, ServiceResponse<AccountDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountCommandHandler(
            ISavingProductRepository savingProductRepository,
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddAccountCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _savingProductRepository = savingProductRepository;
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = UserInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddAccountCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if customer already has an account of this type
                var existingAccount = await _AccountRepository
                    .FindBy(c => c.ProductId == request.ProductId && c.CustomerId == request.CustomerId && c.IsDeleted==false)
                    .FirstOrDefaultAsync();

                // Check if BankId and BranchId fields are set
                if (request.BankId == null || request.BranchId == null)
                {
                    var errorMessage = "Please set the BankId and BranchId fields";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return409(errorMessage);
                }

                // Validate if product exists
                var product = await _savingProductRepository.FindAsync(request.ProductId);
                if (product == null)
                {
                    var errorMessage = "Sorry, this product does not exist";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return409(errorMessage);
                }
                if (request.IsRemoveAccount)
                {

                }
                // Handle account deletion if requested
                if (request.IsRemoveAccount)
                {
                    existingAccount.IsDeleted = true;
                    _AccountRepository.Update(existingAccount);
                    await _uow.SaveAsync();

                    var acc = _mapper.Map<AccountDto>(existingAccount);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Account was deleted successfully.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.ReturnResultWith200(acc, "Account was deleted successfully.");
                }
                
                // If a customer account of the same type exists, return conflict response
                if (existingAccount != null)
                {
                    var errorMessage = "Sorry, you already have an account of this type";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Warning.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return409(errorMessage); // Return 409 conflict
                }

                // Map the AddAccountCommand to an Account entity
                var AccountEntity = _mapper.Map<Account>(request);

                // Set additional properties for the account entity
                AccountEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                AccountEntity.ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                AccountEntity.Id = BaseUtilities.GenerateUniqueNumber();
                string accountNumber = product.AccountNuber == null ? $"{product.Code}" : product.AccountNuber;
                AccountEntity.AccountNumber = BaseUtilities.GenerateAccountNumber(request.CustomerId, accountNumber);
                AccountEntity.AccountType = product.AccountType;
                AccountEntity.AccountName = $"{product.AccountType}";
                AccountEntity.CustomerName = request.CustomerName;

                // Add the new Account entity to the repository
                _AccountRepository.Add(AccountEntity);
                await _uow.SaveAsync();

                // Map the Account entity to AccountDto and return success response
                var AccountDto = _mapper.Map<AccountDto>(AccountEntity);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Customer account created successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.ReturnResultWith200(AccountDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }


























    }

}
