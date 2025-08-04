using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    //public class CheckResponse
    //{
    //    public   bool IsExist { get; set; }
    //    public string  Name { get; set; }
    //}
    //public class AddProductAccountRubriqueCommandHandler : IRequestHandler<AddProductAccountRubriqueCommand, ServiceResponse<bool>>
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly IAccountRepository _accountRepository;
    //    private readonly IProductAccountingBookRepository _productAccountingBookRepository;
    //    private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
    //    private readonly IOperationEventRepository _operationRepository;
    //    private readonly IAccountRubriqueRepository _accountRubriqueRepository;
    //    private readonly IChartOfAccountRepository _chartOfAccountRepository;
    //    private readonly IAccountTypeRepository _accountTypeRepository;
    //    private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
    //    private readonly IMapper _mapper;
    //    private readonly IMediator _mediator;
    //    private readonly ILogger<AddProductAccountRubriqueCommandHandler> _logger;
    //    private readonly IUnitOfWork<POSContext> _uow;
    //    private readonly UserInfoToken _userInfoToken;

    //    public AddProductAccountRubriqueCommandHandler(
    //        IConfiguration configuration,
    //        IProductAccountingBookRepository productAccountingBookRepository,
    //        IChartOfAccountRepository chartOfAccountRepository,
    //        IOperationEventRepository operationRepository,
    //        IAccountTypeRepository accountTypeRepository,
    //        IAccountingRuleEntryRepository accountingRuleEntryRepository,
    //        IAccountRepository accountRepository,
    //        IOperationEventAttributeRepository operationEventAttributeRepository,
    //        IAccountRubriqueRepository accountRubriqueRepository,
    //        IMapper mapper,
    //        UserInfoToken userInfoToken,
    //        ILogger<AddProductAccountRubriqueCommandHandler> logger,
    //        IUnitOfWork<POSContext> uow
    //        )
    //    {
    //        _configuration = configuration;
    //        _chartOfAccountRepository = chartOfAccountRepository;
    //        _accountRubriqueRepository = accountRubriqueRepository;
    //        _operationRepository = operationRepository;
    //        _accountTypeRepository = accountTypeRepository;
    //        _accountRepository = accountRepository;
    //        _operationEventAttributeRepository = operationEventAttributeRepository;
    //        _accountingRuleEntryRepository = accountingRuleEntryRepository;
    //        _productAccountingBookRepository = productAccountingBookRepository;
    //        _mapper = mapper;
    //        _logger = logger;
    //        _uow = uow;
    //        _userInfoToken = userInfoToken;

    //    }

    //    public async Task<ServiceResponse<bool>> Handle(AddProductAccountRubriqueCommand request, CancellationToken cancellationToken)
    //    {
    //        string errorMessage = string.Empty;
    //        try
    //        {
             

    //            var accountTyp = _productAccountingBookRepository.FindBy(x => x.ProductAccountingBookId.Equals(request.ProductAccountingBookId));

    //            if (accountTyp.Count()==0)
    //            {
    //                errorMessage = $"There is no productAccountingBook with productId: {request.ProductAccountingBookId} in the system.";

    //                await LogAndAuditError(errorMessage, request);
    //                return ServiceResponse<bool>.Return422(errorMessage);
    //            } 
    //            var AccountRubriques =   _accountRubriqueRepository.FindBy(x => x.ProductAccountingBookId.Equals(request.ProductAccountingBookId));
    //            if (AccountRubriques.Count()==0)
    //            {
    //                var element = CheckIfAttributeNameExist(request.AccountRubriques, AccountRubriques.ToList());
    //                if (element.IsExist)
    //                {
    //                    errorMessage = $"There already an attribute name :{element} already existing in the system.";
    //                }
    //            }
                


    //            var ProductAccountingBook = accountTyp.FirstOrDefault();
    //            if (ProductAccountingBook == null)
    //            {

    //            }
    //            var balancingAccountId = await GetBalancingAccount(ProductAccountingBook.AccountTypeId);

    //            if (request.AccountRubriques == null || !request.AccountRubriques.Any())
    //            {
    //                errorMessage = $"No OperationEventAttribute has been shared to bind ProductAccountingBook {JsonConvert.SerializeObject(request)}. Kindly share the accounting reference";

    //                _logger.LogError(errorMessage);
    //                await LogAndAuditError(errorMessage, request);
    //                return ServiceResponse<bool>.Return422(errorMessage);
    //            }

    //            var operationEventId = await GetOperationEventId(request.OperationType, ProductAccountingBook.AccountTypeId);

    //            foreach (var item in request.AccountRubriques)
    //            {
    //                var chartOfAccount = await _chartOfAccountRepository.FindAsync(item.ChartOfAccountId);
    //                if (string.IsNullOrEmpty(chartOfAccount.LabelEn) || string.IsNullOrEmpty(chartOfAccount.LabelFr))
    //                {
    //                    var language = string.IsNullOrEmpty(chartOfAccount.LabelFr) ? "French Users" : "English Users";
    //                    errorMessage = $"The ChartOfAccount:{chartOfAccount.AccountNumber}. Does not have label for {language}";
    //                    _logger.LogError(errorMessage);
    //                    await LogAndAuditError(errorMessage, request);
    //                    return ServiceResponse<bool>.Return422(errorMessage);
    //                }
    //                var operationEventAttribute = new OperationEventAttributes
    //                {
    //                    CreatedBy = _userInfoToken.Id,
    //                    ModifiedBy = _userInfoToken.Id,
    //                    CreatedDate = DateTime.Now.ToLocalTime(),
    //                    ModifiedDate = DateTime.Now.ToLocalTime(),
    //                    OperationEventId = operationEventId,
    //                    Id = Guid.NewGuid().ToString(),
    //                    Name = request.OperationType + "@" + item.OperationEventAttributeName,
    //                    Description = item.OperationEventAttributeName,
    //                    DeletedBy = null,
    //                    DeletedDate = DateTime.Now.ToLocalTime(),
    //                    IsDeleted = false
    //                };
    //                var ruleEntry = new Data.AccountingRuleEntry
    //                {
    //                    Id = Guid.NewGuid().ToString(),
    //                    AccountingRuleEntryName = operationEventAttribute.Name,
    //                    BookingDirection = item.IsDebit.Value ? "DEBIT" : "CREDIT",
    //                    CreatedBy = _userInfoToken.Id,
    //                    CreatedDate = DateTime.Now,
    //                    ModifiedBy = _userInfoToken.Id,
    //                    DeletedBy = null,
    //                    IsDeleted = false,
    //                    ModifiedDate = DateTime.Now,
    //                    DeterminationAccountId = await GetDeterminantAccountId(chartOfAccount.Id, operationEventAttribute.Name, ProductAccountingBook.AccountTypeId),
    //                    BankId = "aaaaaa-f8be-490d-b3cb-xxxxxxxxx",
    //                    BalancingAccountId = balancingAccountId,
    //                    EventCode = request.OperationType + "@" + item.OperationEventAttributeName,
    //                    OperationEventAttributeId = operationEventAttribute.Id,
    //                    ProductAccountingBookId = ProductAccountingBook.Id,
    //                };
    //                var accountRubrique = new AccountRubrique
    //                {
    //                    Id = Guid.NewGuid().ToString(),
    //                    CreatedBy = _userInfoToken.Id,
    //                    CreatedDate = DateTime.Now,
    //                    ModifiedBy = _userInfoToken.Id,
    //                    ModifiedDate = DateTime.Now,
    //                    DeletedBy = null,
    //                    DeletedDate = null,
    //                    IsDeleted = false,
    //                    AccountTypeId = ProductAccountingBook.AccountTypeId,
    //                    ChartOfAccountId = chartOfAccount.Id,
    //                    OperationEventAttributeId = operationEventAttribute.Id,                     
    //                    Name =   item.OperationEventAttributeName,
    //                    ProductAccountingBookId = request.ProductAccountingBookId
    //                };
    //                _operationEventAttributeRepository.Add(operationEventAttribute);
    //                _accountRubriqueRepository.Add(accountRubrique);
    //                _accountingRuleEntryRepository.Add(ruleEntry);
    //            }

    //            await _uow.SaveAsync();
                

    //            return ServiceResponse<bool>.ReturnResultWith200(true);
    //        }
    //        catch (Exception e)
    //        {
    //            errorMessage = $"Error occurred while saving AccountRubrique: {e.Message}";

    //            _logger.LogError(errorMessage);
    //            await LogAndAuditError(errorMessage, request);

    //            return ServiceResponse<bool>.Return500(e, errorMessage);
    //        }
    //    }

    //    private CheckResponse CheckIfAttributeNameExist(List<ProductAccountRubric> accountRubriques, List<AccountRubrique> accountTyp)
    //    {
    //        if (accountRubriques == null || accountTyp == null || accountRubriques.Count == 0 || accountTyp.Count == 0)
    //        {
    //            return new CheckResponse { IsExist = false }; // No need to proceed if either list is null or empty
    //        }

    //        var attributeNameSet = new HashSet<string>(accountRubriques.Select(r => r.OperationEventAttributeName));
    //        bool isExist = false;
    //        string attributeName = null;

    //        foreach (var item in accountTyp)
    //        {
    //            if (attributeNameSet.Contains(item.Name))
    //            {
    //                isExist = true;
    //                attributeName = item.Name;
    //                break;
    //            }
    //        }

    //        return new CheckResponse
    //        {
    //            Name = attributeName,
    //            IsExist = isExist
    //        };
    //    }

    //    private string GetAttributeNameExisting(List<ProductAccountRubric> accountRubriques, IQueryable<ProductAccountingBook> accountTyp)
    //    {
    //        // Get list of attribute names from first list
    //        var attributeNames = accountRubriques.Select(x => x.OperationEventAttributeName).ToList();

    //        // Find first match in second list
    //        var match = accountTyp.FirstOrDefault(x => attributeNames.Contains(x.Name));

    //        // Return matching name if found, null otherwise
    //        return match?.Name;
    //    }
    //    private async Task<string> GetDeterminantAccountId(string id, string name, string accountTypeId)
    //    {
    //        string Id = "";
    //        var acccountIdXXX = await _chartOfAccountRepository.FindAsync(id);

    //        if (acccountIdXXX != null)
    //        {
    //            var accountId = _accountRepository.FindBy(c => c.ChartOfAccountId == acccountIdXXX.Id && c.AccountOwnerId == _userInfoToken.Id);

    //            if (accountId.Any())
    //            {
    //                Id = accountId.FirstOrDefault().Id;
    //            }
    //            else
    //            {
    //                //var bankInfo = await APICallHelper.GetBankInfo(new PathHelper(_configuration), _userInfoToken);

    //                AccountCommand command = new AccountCommand
    //                {
    //                    AccountHolder = _userInfoToken.BranchId+ " " + name,
    //                    AccountNumber = acccountIdXXX.AccountNumber,
    //                    AccountOwnerId = _userInfoToken.Id,
    //                    AccountTypeId = accountTypeId,
    //                    ChartOfAccountId = acccountIdXXX.Id,
    //                };
    //                var AccModel =  CreateAccount(command);
    //                _accountRepository.Add(AccModel);
    //                    Id = AccModel.Id;
              
    //            }
    //        }
    //        return Id;
    //    }
       
    //    private async Task<string> GetBalancingAccount(string accountTypeId)
    //    {
    //        string Id = "";
    //        var balancingAccountId = _chartOfAccountRepository.FindBy(x => x.AccountNumber.Equals("1"));

    //        if (balancingAccountId.Any())
    //        {
    //            string Idstr = balancingAccountId.FirstOrDefault().Id;
    //            var accountId = _accountRepository.FindBy(c => c.ChartOfAccountId == Idstr);

    //            if (accountId.Any())
    //            {
    //                Id = accountId.FirstOrDefault().Id;
    //            }
    //            else
    //            {
    //               // var bankInfo = await APICallHelper.GetBankInfo(new PathHelper(_configuration), _userInfoToken);

    //                var command = new AddAccountCommand
    //                {
    //                    AccountHolder = _userInfoToken.BranchId + " " + "Banlancing Account",
    //                    AccountNumber = "1",
    //                    AccountOwnerId = _userInfoToken.Id,
    //                    AccountTypeId = accountTypeId,
    //                    ChartOfAccountId = Idstr,
    //                };

    //                await _mediator.Send(command);

    //                accountId = _accountRepository.FindBy(c => c.ChartOfAccountId == Idstr && c.AccountOwnerId == _userInfoToken.Id);

    //                if (accountId != null)
    //                {
    //                    Id = accountId.FirstOrDefault().Id;
    //                }
    //            }
    //        }
    //        return Id;
    //    }

    //    private async Task<string> GetOperationEventId(string operationType,string AccountTypeId)
    //    {
    //        var modelOp =   _operationRepository.FindBy(x => x.OperationEventName.ToUpper() == operationType.ToUpper());

    //        if (modelOp.Any())
    //        {
    //            return modelOp.FirstOrDefault().Id;
    //        }
    //        else
    //        {
    //            var operationEventId = Guid.NewGuid().ToString();

    //            _operationRepository.Add(new OperationEvent
    //            {
    //                AccountTypeId =  AccountTypeId,
    //                OperationEventName = operationType.ToString(),
    //                EventCode = operationType.ToString(),
    //                CreatedBy = _userInfoToken.Id,
    //                CreatedDate = DateTime.Now,
    //                ModifiedBy = _userInfoToken.Id,
    //                ModifiedDate = DateTime.Now,
    //                IsDeleted = false,
    //                DeletedBy = _userInfoToken.Id,
    //                DeletedDate = null,
    //                Description = operationType.ToString(),
    //                Id = operationEventId,
    //            });

    //            return operationEventId;
    //        }
    //    }

    //    private async Task LogAndAuditError(string errorMessage, AddProductAccountRubriqueCommand request)
    //    {
    //        await APICallHelper.AuditLogger(_userInfoToken.Email, "AddProductAccountRubriqueCommand", JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //    }
    //    private Data.Account CreateAccount(AccountCommand model)
    //    {
    //        Data.Account account = new Data.Account();
    //        account.AccountHolder = model.AccountHolder;
    //        account.AccountTypeId = model.AccountTypeId;
    //        account.ChartOfAccountId = model.ChartOfAccountId;
    //        account.AccountNumber = model.AccountNumber;
    //        account.AccountOwnerId = model.AccountOwnerId;
    //        account.Id = Guid.NewGuid().ToString();
    //        account.CreatedBy = _userInfoToken.Id;
    //        account.ModifiedBy = null;
    //        account.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
    //        account.ModifiedDate = BaseUtilities.UtcToDoualaTime(new DateTime());
    //        account.IsDeleted = false;
    //        account.DeletedBy = null;
    //        account.DeletedDate = null;
    //        account.Status = AccountStatus.Active.ToString();
    //        return account;
    //    }
    //}
}
