using AutoMapper;
using Azure;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountType.
    /// </summary>
    public class AddProductAccountingBookCommandHandler : IRequestHandler<AddProductAccountingBookCommand, ServiceResponse<ProductAccount>>
    {
        private readonly IOperationEventRepository _operationRepository; // Repository for accessing Operation data.
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
        private readonly IProductAccountingBookRepository _productAccountingBookRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
 
        private readonly IAccountRepository _accountRepository; // Repository for accessing Operation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddProductAccountingBookCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly UserInfoToken _userInfoToken;
        private readonly string _balancingAccount  = "ROOT ACCOUNT";
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        //private BranchInfo bankInfox;

        /// <summary>
        /// Constructor for initializing the AddAccountTypeCommandHandler.
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
        public AddProductAccountingBookCommandHandler(
                IAccountTypeRepository AccountTypeRepository,
                IProductAccountingBookRepository productAccountingBookRepository,
                IAccountRepository accountRepository,
                IAccountingRuleEntryRepository accountingRuleEntryRepository,
        IChartOfAccountRepository chartOfAccountRepository,
                IOperationEventRepository operationRepository,
                     IOperationEventAttributeRepository operationEventAttributeRepository,
        IMapper mapper,
                UserInfoToken userInfoToken,
                ILogger<AddProductAccountingBookCommandHandler> logger,
                IUnitOfWork<POSContext> uow, IMediator mediator, IConfiguration configuration, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _accountRepository = accountRepository;
            _productAccountingBookRepository = productAccountingBookRepository;
            _chartOfAccountRepository= chartOfAccountRepository;
             _AccountTypeRepository = AccountTypeRepository;
            _operationRepository = operationRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
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
        public async Task<ServiceResponse<ProductAccount>> Handle(AddProductAccountingBookCommand request, CancellationToken cancellationToken)
        {
  
            List<AccountNotUpdated> list = new List<AccountNotUpdated>();
            string errorMessage = "";
            string AccountTypeId = Guid.NewGuid().ToString();
            try
            {

                var AccountTypeModel = _AccountTypeRepository.FindBy(c => c.OperationAccountType == CheckAccountSystemType(request.ProductAccountBookType));
                if (AccountTypeModel.Any())
                {
                    AccountTypeId = AccountTypeModel.FirstOrDefault().Id;
                }
                else
                {
                      errorMessage = $"No operational or Ordinary account has been set";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.AddProductAccountingBookCommand, LogLevelInfo.Warning);
                    return ServiceResponse<ProductAccount>.Return409(errorMessage);
                }

                var ProductExist = await _productAccountingBookRepository.FindBy(c => c.ProductAccountingBookId == request.ProductAccountBookId).FirstOrDefaultAsync();
                if (ProductExist!=null)
                {
  
                  var listprod =   _accountingRuleEntryRepository.FindBy(x => x.EventCode.Contains(request.ProductAccountBookId));

                    var  Items = GetAccountingCOnfigurationTobeModified(listprod.ToList(), request.ProductAccountBookDetails,request.ProductAccountBookId);

                    if (Items.Item1.Count()>0)
                    {
                        var Command = new UpdateProductAccountingBookCommand
                        {
                            ProductId = request.ProductAccountBookId,
                            ProductName = request.ProductAccountBookName,
                            ProductAccountBookDetails = Items.Item1
                        };
                        var result = await _mediator.Send(Command);
                    }
                 
               
                    if (Items.Item2.Count()==0) 
                    {
                        errorMessage = $"All ProductAccountingBook for {request.ProductAccountBookName} has been updated successfully.";
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.AddProductAccountingBookCommand, LogLevelInfo.Warning);
                        if (request.ProductAccountBookType == "Loan_Product")
                        {

                            var modelAccountRuleEntry = await _productAccountingBookRepository.FindAsync($"{request.ProductAccountBookId}@Loan_Principal_Account");

                            var chartOfAccountMP = await _chartOfAccountManagementPositionRepository.FindAsync(modelAccountRuleEntry.ChartOfAccountManagementPositionId);
                            return ServiceResponse<ProductAccount>.ReturnResultWith200(new ProductAccount { AccountNumber = chartOfAccountMP.AccountNumber.PadRight(6, '0'), ChartOfAccountManagementPositionId = chartOfAccountMP.Id });

                        }
                        else
                        {
                            var modelAccountRuleEntry = await _productAccountingBookRepository.FindAsync($"{request.ProductAccountBookId}@Principal_Saving_Account");

                            var chartOfAccountMP = await _chartOfAccountManagementPositionRepository.FindAsync(modelAccountRuleEntry.ChartOfAccountManagementPositionId);
                            return ServiceResponse<ProductAccount>.ReturnResultWith200(new ProductAccount { AccountNumber = chartOfAccountMP.AccountNumber.PadRight(6, '0'), ChartOfAccountManagementPositionId = chartOfAccountMP.Id, notUpdateds = list }, errorMessage);

                        }
                    }
                    else
                    {
                        request.ProductAccountBookDetails = Items.Item2;
                    }
   

                }
                List<ProductAccountingBookDto> productAccountingBookList = new List<ProductAccountingBookDto>();


                var AccountTypeDetailEntities = _mapper.Map<List<Data.AccountTypeDetail>>(request.ProductAccountBookDetails);
                string OperationEventId = Guid.NewGuid().ToString();

           
                 
                    var AccountingBookAttribute = ProductAccountingBook.productAccountingBooking(AccountTypeDetailEntities, AccountTypeId, request.ProductAccountBookId, request.ProductAccountBookName, request.ProductAccountBookType, AccountTypeId, _userInfoToken, OperationEventId);
                    productAccountingBookList = _mapper.Map(AccountingBookAttribute.ProductAccountingBooksList, productAccountingBookList);
                    var modelOperationEvent = await GetOperationEventId(request.ProductAccountBookName, request.ProductAccountBookType, AccountTypeId);
                   List<AccountCommand> listOfAccount = AddAccountCommandList.CreateListOfAccountwithAccountType(AccountTypeDetailEntities, AccountTypeId, request.ProductAccountBookName, _userInfoToken);
                    List<OperationEventAttributes> listOfOperationEventAttributes = new List<OperationEventAttributes>();
                    List<Data.AccountingRuleEntry> listOfAccountingRuleEntry = new List<Data.AccountingRuleEntry>();
                    Data.ChartOfAccountManagementPosition BalancingAccount = _chartOfAccountManagementPositionRepository.All.Where(c => c.Description.ToLower() == _balancingAccount.ToLower() && c.IsDeleted == false).FirstOrDefault();        
                    foreach (var item in AccountTypeDetailEntities)
                    {
                        var operationEventAttribute = new OperationEventAttributes
                        {
                            CreatedBy = _userInfoToken.Id,
                            ModifiedBy = _userInfoToken.Id,
                            CreatedDate = DateTime.Now.ToLocalTime(),
                            ModifiedDate = DateTime.Now.ToLocalTime(),
                            OperationEventId = modelOperationEvent.OperationEvent.Id,
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "Env-"),
                            Name = request.ProductAccountBookName + "@" + item.Name,
                            Description = request.ProductAccountBookName + "@" + item.Name,
                            OperationEventAttributeCode= request.ProductAccountBookName + "@" + item.Name,
                            DeletedBy = null,
                            DeletedDate = DateTime.Now.ToLocalTime(),
                            IsDeleted = false
                        };
                        listOfOperationEventAttributes.Add(operationEventAttribute);
                        await CreateDeterminantAccountId(item.ChartOfAccountId, AccountTypeId);
                        var ruleEntry = new Data.AccountingRuleEntry
                        {
                          
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "Env-"),
                            AccountingRuleEntryName = item.Name,
                            BookingDirection = GetBookingDirection(item.Name),
                            CreatedBy = _userInfoToken.Id,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = _userInfoToken.Id,
                            DeletedBy = null,
                            IsDeleted = false,
                            ModifiedDate = DateTime.Now,
                            DeterminationAccountId = item.ChartOfAccountId,
                            BankId = _userInfoToken.BankId,
                            BalancingAccountId = BalancingAccount.Id,
                            EventCode = request.ProductAccountBookId + "@" + item.Name,
                            OperationEventAttributeId = operationEventAttribute.Id,
                            ProductAccountingBookId = request.ProductAccountBookId + "@" + item.Name,
                        };
                        listOfAccountingRuleEntry.Add(ruleEntry);
                    }
                    if (modelOperationEvent.Exist == false)
                        _operationRepository.Add(modelOperationEvent.OperationEvent);
         
                _operationEventAttributeRepository.AddRange(listOfOperationEventAttributes);
                _accountingRuleEntryRepository.AddRange(listOfAccountingRuleEntry);
                _productAccountingBookRepository.AddRange(AccountingBookAttribute.ProductAccountingBooksList);

                await _uow.SaveAsync();
                  errorMessage=  $"new AccountType created Successfully.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.AddProductAccountingBookCommand, LogLevelInfo.Information);
    

                if (request.ProductAccountBookType== "Loan_Product")
                {
                    
                         var modelAccountRuleEntry = await _productAccountingBookRepository.FindAsync($"{request.ProductAccountBookId}@Loan_Principal_Account");

                    var chartOfAccountMP = await _chartOfAccountManagementPositionRepository.FindAsync(modelAccountRuleEntry.ChartOfAccountManagementPositionId);
                    return ServiceResponse<ProductAccount>.ReturnResultWith200(new ProductAccount { AccountNumber = chartOfAccountMP.AccountNumber.PadRight(6,'0'), ChartOfAccountManagementPositionId = chartOfAccountMP.Id });

                }
                else
                {
                    var modelAccountRuleEntry = await _productAccountingBookRepository.FindAsync($"{request.ProductAccountBookId}@Principal_Saving_Account");

                    var chartOfAccountMP = await _chartOfAccountManagementPositionRepository.FindAsync(modelAccountRuleEntry.ChartOfAccountManagementPositionId);
                    return ServiceResponse<ProductAccount>.ReturnResultWith200(new ProductAccount { AccountNumber = chartOfAccountMP.AccountNumber.PadRight(6, '0'), ChartOfAccountManagementPositionId = chartOfAccountMP.Id, notUpdateds = list }, errorMessage);

                }

            }
            catch (Exception exception)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = JsonConvert.SerializeObject(exception);
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AddProductAccountingBookCommand, LogLevelInfo.Error);


                return ServiceResponse<ProductAccount>.Return500(exception, errorMessage);
            }

        }

        private (List<ProductAccountBookDetail>, List<ProductAccountBookDetail>) GetAccountingCOnfigurationTobeModified(List<AccountingRuleEntry> AccountingRuleEntries, List<ProductAccountBookDetail> productAccountBookDetails ,string productId)
        {
            List<ProductAccountBookDetail> detailTobeUpdated = new List<ProductAccountBookDetail>();
            List<ProductAccountBookDetail> detailTobeAdded = new List<ProductAccountBookDetail>();
            foreach (var entryRule in productAccountBookDetails)
            {
                var model = AccountingRuleEntries.Where(x => x.EventCode == productId + "@" + entryRule.Name);
                if (model.Any())
                {
                    var entry = model.FirstOrDefault();
                    if (entry != null)
                    {
                        if (entry.DeterminationAccountId == entryRule.ChartOfAccountId)
                        {
                            continue;
                        }
                        else
                        {
                            detailTobeUpdated.Add(entryRule);
                        }
                    }
  
                }
                else
                {
                    detailTobeAdded.Add(entryRule);
                }
            }
 
            return (detailTobeUpdated, detailTobeAdded);
        }

        public string GetInnerExceptionMessages(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var innerMessage = "";

            if (exception.InnerException != null)
            {
                innerMessage = "\n" + GetInnerExceptionMessages(exception.InnerException);
            }

            return exception.Message + innerMessage;
        }

        private async Task CreateDeterminantAccountId(string chartOfAccountManagementPositionId,  string accountTypeId)
        {
            Data.Account data = new Data.Account();
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(chartOfAccountManagementPositionId);

            if (chartOfAccount != null)
            {
                //var bankInfo = await APICallHelper.GetBankInfo(new PathHelper(_configuration), _userInfoToken);
                var accountId = _accountRepository.All.Where(c => c.ChartOfAccountManagementPositionId == chartOfAccount.Id && c.AccountOwnerId == _userInfoToken.BranchId);

                if (accountId.Any())
                {
                    data = accountId.FirstOrDefault();
                }
                else
                {
                    var AccountManagementPosition = chartOfAccount.PositionNumber.PadRight(3,'0');
                    chartOfAccount.ChartOfAccount = await _chartOfAccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
                    
                    AddAccountCommand command = new AddAccountCommand
                    {
                        AccountName = chartOfAccount.Description + " " + _userInfoToken.BranchCode,
                        AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                        AccountNumberManagementPosition = AccountManagementPosition,
                        AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + _userInfoToken.BranchCode).PadRight(12, '0')+AccountManagementPosition,
                        AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode).PadRight(9, '0')+AccountManagementPosition,
                        AccountOwnerId = _userInfoToken.BranchId,
                        AccountTypeId = accountTypeId,
                        ChartOfAccountManagementPositionId = chartOfAccount.Id,
                        OwnerBranchCode = _userInfoToken.BranchCode, 
                        AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
                    };
                     await _mediator.Send(command);
      

    }
            }
       
        }
        public string GetAccountNumberManagementPosition(Data.ChartOfAccount chartOfAccount)
        {
            string numberString = string.Empty;
            if (chartOfAccount.HasManagementPostion.Value)
            {
                var models = _chartOfAccountManagementPositionRepository.FindBy(f => f.ChartOfAccountId == chartOfAccount.Id);

                if (models.Any())
                {
                    numberString = models.FirstOrDefault().PositionNumber.PadRight(3, '0');
                }
            }
            return numberString;
        }

        private string GetBookingDirection(string accountName)
        {

            if (accountName.Contains("Expense_Account"))
            {
                return "DEBIT";
            }


            if (accountName.Contains("Saving_Account"))
            {
                return "CREDIT";
            }

            if (accountName.Contains("Interest_Account"))
            {
                return "DEBIT";
            }

            return "Not Found";
        }


        private async Task<ConfirmOperations> GetOperationEventId(string ProductName, string operationType, string AccountTypeId)
        {
            ConfirmOperations operations = new ConfirmOperations();
            var modelOp = _operationRepository.FindBy(x => x.OperationEventName.ToUpper() == ProductName.ToUpper());

            if (modelOp.Any())
            {
                operations.OperationEvent = modelOp.FirstOrDefault();
                operations.Exist = true;
            }
            else
            {
                var operationEventId = BaseUtilities.GenerateInsuranceUniqueNumber(10,"OE");

                operations.OperationEvent = new OperationEvent
                {
                    AccountTypeId = AccountTypeId,
                    OperationEventName = ProductName,
                    EventCode = operationType.ToString(),
                    CreatedBy = _userInfoToken.Id,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = _userInfoToken.Id,
                    ModifiedDate = DateTime.Now,
                    IsDeleted = false,
                    DeletedBy = _userInfoToken.Id,
                    DeletedDate = default,
                    Description = operationType.ToString(),
                    Id = operationEventId,
                };
                operations.Exist = false;

            }
            return operations;
        }

        private string CheckAccountSystemType(string operationAccountType)
        {
            if (operationAccountType.ToUpper() == AccountType_Product.Loan_Product.ToString().ToUpper() || operationAccountType.ToUpper() == AccountType_Product.Saving_Product.ToString().ToUpper())
            {
                return "ORD";
            }
            else
            {
                return "OPE";
            }
        }
        private bool CheckIfSAVINGPRODUCT(string operationAccountType)
        {
            return operationAccountType.ToUpper() == AccountType_Product.Saving_Product.ToString().ToUpper();
        }

        private bool CheckIfTellerAccount(string operationAccountType)
        {
            return operationAccountType.ToUpper() == AccountType_Product.Teller.ToString().ToUpper();
        }
        private bool CheckIfLoanAccount(string operationAccountType)
        {
            return operationAccountType.ToUpper() == AccountType_Product.Loan_Product.ToString().ToUpper();
        }
        //private async Task GetAuxilaryObject()
        //{
        //    this.bankInfox = await APICallHelper.GetBankInfo(new PathHelper(_configuration), _userInfoToken);


        //}

        private List<RuleEntry> GenerateEntryAccountingBookRule(List<ProductAccountingBook> productAccountingBooks)
        {
            List<RuleEntry> ruleEntries = new List<RuleEntry>();
            foreach (var item in productAccountingBooks)
            {
                ruleEntries.Add(new RuleEntry
                {
                    AccountingRuleEntryName = item.ProductAccountingBookName,
                    DeterminationAccountId = item.ChartOfAccountId,
                    BalancingAccountId = null,
                    AccountTypeId = item.AccountTypeId,
                    OperationEventAttributeId = item.OperationEventAttributeId,
                    IsPrimaryEntry = false
                });
            }
            return ruleEntries;
        }
    }


    public class ConfirmOperations
    {
        public bool Exist { get; set; }
        public OperationEvent OperationEvent { get; set; }
    }
}