using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddTellerAccountCommandHandler : IRequestHandler<AddTellerAccountCommand, ServiceResponse<AccountDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTellerAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ISavingProductRepository _savingProductRepository; // Repository for accessing Account data.

        /// <summary>
        /// Constructor for initializing the AddTellerAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTellerAccountCommandHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddTellerAccountCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            ISavingProductRepository savingProductRepository)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = UserInfoToken;
            _uow = uow;
            _savingProductRepository = savingProductRepository;
        }

        /// <summary>
        /// Handles the AddTellerAccountCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddTellerAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(AddTellerAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //check if bankId and BranchId fields are set
                if (request.BankId == null||request.BranchId==null) {
                    var errorMessage = $"Please set the BankId and BranchId fields";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email,LogAction.Create.ToString(), request, errorMessage,LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return409(errorMessage);
                }

                //check if you teller already has an account of this type
                //check if you teller already has an account of this type
                var existingAccount = await _AccountRepository.FindBy(c => c.ProductId == request.ProductId && c.TellerId == request.TellerId)
                    .Include(x => x.Product).FirstOrDefaultAsync();

                // If a teller account of the same type exists
                if (existingAccount != null)
                {
                    var errorMessage = $"Sorry you already have a {existingAccount.Product.Name} account";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return409(errorMessage);
                }
                var product = await _savingProductRepository.FindAsync(request.ProductId);

                // Check if the product is null
                if (product == null)
                {
                    var errorMessage = $"Product with ID {request.ProductId} was not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return404(errorMessage); // Return 404 or any appropriate error code
                }

                // Map the AddTellerAccountCommand to a Account entity
                var AccountEntity = _mapper.Map<Account>(request);
                // Convert UTC to local time and set it in the entity
                AccountEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                AccountEntity.Status = AccountStatus.Closed.ToString();
                AccountEntity.Id = BaseUtilities.GenerateUniqueNumber();
                string accountNumber = product.AccountNuber == null ? $"{product.Code}" : product.AccountNuber;
                AccountEntity.AccountNumber = $"{accountNumber}{product.Code}";
                // Add the new Account entity to the repository
                _AccountRepository.Add(AccountEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "An error occurred creating the teller's account", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return500();
                }
                // Map the Account entity to AccountDto and return it with a success response
                var AccountDto = _mapper.Map<AccountDto>(AccountEntity);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "teller account created successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
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
