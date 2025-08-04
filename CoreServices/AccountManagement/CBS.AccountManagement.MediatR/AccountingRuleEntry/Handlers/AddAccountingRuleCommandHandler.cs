using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddAccountingRuleEntryCommandHandler : IRequestHandler<AddAccountingRuleEntryCommand, ServiceResponse<AccountingRuleEntryDto>>
    {


        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountingRuleEntryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IOperationEventAttributeRepository _eventAttributeRepository;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IChartOfAccountRepository _accountRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _madiator;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository; // Repository for accessing Account data.


        /// <summary>
        /// Constructor for initializing the AddAccountingRuleEntryCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountingRuleEntryCommandHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository, IOperationEventAttributeRepository eventAttributeRepository, IOperationEventRepository operationEventRepository,
       IMapper mapper,
            ILogger<AddAccountingRuleEntryCommandHandler> logger,
            IUnitOfWork<POSContext> uow, IChartOfAccountRepository? accountRepository, IMediator? madiator, UserInfoToken? userInfoToken, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _eventAttributeRepository = eventAttributeRepository;
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _operationEventRepository = operationEventRepository;
            _mapper = mapper;
            _logger = logger;
            _accountRepository = accountRepository;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _madiator = madiator;
        }

        /// <summary>
        /// Handles the AddAccountingRuleEntryCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountingRuleEntryCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountingRuleEntryDto>> Handle(AddAccountingRuleEntryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Account with the same name already exists (case-insensitive)
                var existingAccount = await _AccountingRuleEntryRepository.FindBy(c => c.AccountingRuleEntryName == request.AccountingRuleEntryName|| c.OperationEventAttributeId == request.OperationEventAttributeId).FirstOrDefaultAsync();

                // If a Account with the same name already exists, return a conflict response
                if (existingAccount != null)
                {
                    var errorMessage = $"AccountingRuleEntry {request.AccountingRuleEntryName} or {existingAccount.EventCode}already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountingRuleEntryDto>.Return409(errorMessage);
                }
                var existingRule = _AccountingRuleEntryRepository.FindBy(f => f.OperationEventAttributeId == request.OperationEventAttributeId);

                if (existingRule.Any())
                {
                    var errorMessage = $"AccountingRuleEntry {request.AccountingRuleEntryName} event attribute already y exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountingRuleEntryDto>.Return409(errorMessage);
                }
                // Retrive the operational eventCode 
                var eventAttributes = await _eventAttributeRepository.FindAsync(request.OperationEventAttributeId);


                if (eventAttributes == null)
                {
                    var errorMessage = $"No operational event for  {request.OperationEventAttributeId} attribute exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountingRuleEntryDto>.Return409(errorMessage);
                }
                var eventoprationID = await _operationEventRepository.All.Where(x => x.Id.Equals(eventAttributes.OperationEventId)).FirstOrDefaultAsync();
                // Map the AddAccountingRuleEntryCommand to a AccountingRuleEntry entity
                //var chartOfAccount0 = await _accountRepository.FindAsync(request.BalancingAccountId);
                var determinant = await GetChartOfAccountManagementPositionAsync(request.DeterminationAccountId);
                var Balancing = await GetChartOfAccountManagementPositionAsync(request.BalancingAccountId);
                var Account =  GetPrepareAccountAndCreateAsync(determinant).Result;
                var Account1 = GetPrepareAccountAndCreateAsync(Balancing).Result;


                var AccountingRuleEntryEntity = _mapper.Map<Data.AccountingRuleEntry>(request);

                AccountingRuleEntryEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "ARE");
                AccountingRuleEntryEntity.EventCode = eventoprationID.EventCode;
                AccountingRuleEntryEntity.BookingDirection = request.BookingDirection.ToUpper();
                // Add the new Account entity to the repository
                _AccountingRuleEntryRepository.Add(AccountingRuleEntryEntity);
                await _uow.SaveAsync();

                // Map the Account entity to AccountingRuleEntryDto and return it with a success response
                var AccountingRuleEntryDto = _mapper.Map<AccountingRuleEntryDto>(AccountingRuleEntryEntity);
                return ServiceResponse<AccountingRuleEntryDto>.ReturnResultWith200(AccountingRuleEntryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountingRuleEntryDto>.Return500(e);
            }
        }

        private async Task<ServiceResponse<bool>> GetPrepareAccountAndCreateAsync(ChartOfAccountManagementPosition modell)
        {
          
            modell.ChartOfAccount = await _accountRepository.FindAsync(modell.ChartOfAccountId); 
            var model= new AddAccountCommand
            {
                AccountNumberManagementPosition = modell.PositionNumber,
                AccountName = modell.Description + " " + _userInfoToken.BranchName,
                AccountNumber = modell.ChartOfAccount.AccountNumber,
          
                AccountNumberNetwok = (modell.ChartOfAccount.AccountNumber.PadRight(6, '0') + modell.PositionNumber.PadRight(3, '0') + _userInfoToken.BankCode + _userInfoToken.BranchCode).PadRight(12, '0') ,
                AccountNumberCU = (modell.ChartOfAccount.AccountNumber.PadRight(6, '0') + modell.PositionNumber.PadRight(3, '0') + _userInfoToken.BranchCode).PadRight(9, '0'),
                AccountCategoryId = modell.ChartOfAccount.AccountCartegoryId,
                AccountTypeId = "",
                AccountOwnerId = modell.ChartOfAccount.BranchId,
                ChartOfAccountManagementPositionId = modell.Id,
                OwnerBranchCode = _userInfoToken.BranchCode,
                IsNormalCreation = false,
   
            };
           return  await _madiator.Send(model);
        }

        public async Task<ChartOfAccountManagementPosition> GetChartOfAccountManagementPositionAsync(string chartOfAccountManagementId)
        {
            return await _chartOfAccountManagementPositionRepository.FindAsync(chartOfAccountManagementId);



        }
    }

}