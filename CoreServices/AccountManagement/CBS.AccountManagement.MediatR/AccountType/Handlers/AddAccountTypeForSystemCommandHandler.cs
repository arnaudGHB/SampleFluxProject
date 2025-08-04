using AutoMapper;
using Azure;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountType.
    /// </summary>
    public class AddAccountTypeForSystemCommandHandler : IRequestHandler<AddAccountTypeForSystemCommand, ServiceResponse<AccountTypeDto>>
    {
        private readonly IOperationEventRepository _operationRepository; // Repository for accessing Operation data.
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
        private readonly IProductAccountingBookRepository _productAccountingBookRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly ITransactionDataRepository _accountRepository; // Repository for accessing Operation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountTypeForSystemCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly UserInfoToken _userInfoToken;
        //private ServiceResponse<BranchInfo> bankInfo;

        /// <summary>
        /// Constructor for initializing the AddAddAccountTypeForSystemCommandHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="productAccountingBookRepository">Repository for ProductAccountingBook data access.</param>
        /// <param name="accountRepository">Repository for Account data access.</param>
        /// <param name="chartOfAccountRepository">Repository for ChartOfAccount data access.</param>
        /// <param name="operationRepository">Repository for Operation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">User information token.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of work for POSContext.</param>
        /// <param name="mediator">Mediator for handling MediatR requests.</param>
        /// <param name="configuration">Configuration for the application.</param>
        public AddAccountTypeForSystemCommandHandler(
                IAccountTypeRepository AccountTypeRepository,
                IProductAccountingBookRepository productAccountingBookRepository,
                ITransactionDataRepository accountRepository,
                ICashMovementTrackingConfigurationRepository chartOfAccountRepository,
                IOperationEventRepository operationRepository,
                IMapper mapper,
                UserInfoToken userInfoToken,
                ILogger<AddAccountTypeForSystemCommandHandler> logger,
                IUnitOfWork<POSContext> uow, IMediator mediator, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _productAccountingBookRepository = productAccountingBookRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _AccountTypeRepository = AccountTypeRepository;
            _operationRepository = operationRepository;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
        }

        public OperationEvent? attObject { get; private set; }

        /// <summary>
        /// Handles the AddAccountTypeCommand to add a new AccountType.
        /// </summary>
        /// <param name="request">The AddAccountTypeCommand containing AccountType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountTypeDto>> Handle(AddAccountTypeForSystemCommand request, CancellationToken cancellationToken)
        {
            string AccountTypeId = Guid.NewGuid().ToString();
         
            try
            {
                // Check if a AccountType with the same name already exists (case-insensitive)
                var existingAccountType = await _AccountTypeRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a AccountType with the same name already exists, return a conflict response
                if (existingAccountType != null)
                {
                    var errorMessage = $"AccountType name :{request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountTypeCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<AccountTypeDto>.Return409(errorMessage);
                }
                // Map the AddAccountTypeForSystemCommand to a AccountType entity
                var AccountTypeEntity = new AccountType { 
                OperationAccountType = request.Code,
                OperationAccountTypeId= request.Description,
                Name= request.Name,
                 Id =AccountTypeId 
                };
       

                // Add the new AccountType entity to the repository
                _AccountTypeRepository.Add(AccountTypeEntity);
                await _uow.SaveAsync();

                var errorMessag = $"new AccountType created Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommand",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<AccountTypeDto>.ReturnResultWith200(_mapper.Map<AccountTypeDto>(AccountTypeEntity));
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountType: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountTypeCommand",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AccountTypeDto>.Return500(e,errorMessage);
            }

        }
    
        private List<RuleEntry> GenerateEntryAccountingBookRule(List<ProductAccountingBook> productAccountingBooks)
        {
            List < RuleEntry > ruleEntries = new List<RuleEntry>();
            foreach (var item in productAccountingBooks)
            {
                ruleEntries.Add(new RuleEntry { 
                 AccountingRuleEntryName=item.ProductAccountingBookName,
                 DeterminationAccountId = item.ChartOfAccountId,
                 BalancingAccountId=null,
                 AccountTypeId=item.AccountTypeId,
                 OperationEventAttributeId= item.OperationEventAttributeId,
                    IsPrimaryEntry = false
                });
            }
            return ruleEntries;
        }
    }

 
}