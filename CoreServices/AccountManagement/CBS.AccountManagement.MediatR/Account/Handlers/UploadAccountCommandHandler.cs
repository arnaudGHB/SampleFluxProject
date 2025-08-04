using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data.Entity.Currency;
using CBS.AccountManagement.Data.Enum;
using Microsoft.AspNetCore.Hosting;
using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CBS.AccountManagement.MediatR.Handlers
{

    public class UploadAccountCommandHandler : IRequestHandler<UploadAccount, ServiceResponse<UploadAccountResult>>
    {
        // Dependencies
        private readonly IAccountRepository _accountRepository;

        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly ITrailBalanceUploudRepository _trailBalanceUploudRepository;
        private readonly ILogger<UploadAccountCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _madiator;
        private readonly PathHelper _pathHelper;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly FileProcessor _fileProcessor;
        public string BranchId { get; private set; }

        // Constructor to inject dependencies
        public UploadAccountCommandHandler(IWebHostEnvironment IWebHostEnvironment, PathHelper pathHelper, IAccountRepository accountRepository, ILogger<UploadAccountCommandHandler> logger, IMapper mapper, UserInfoToken userInfoToken, IChartOfAccountRepository? chartOfAccountRepository, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository, IUnitOfWork<POSContext>? uow, IMediator? madiator, IAccountingEntryRepository? accountingEntryRepository, ITrailBalanceUploudRepository? trialBalance4columnRepository)
        {
            _fileProcessor = new FileProcessor(IWebHostEnvironment, pathHelper);
            _ChartOfAccountRepository = chartOfAccountRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _accountRepository = accountRepository;
            _logger = logger;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _madiator = madiator;
            _pathHelper = pathHelper;
            _trailBalanceUploudRepository = trialBalance4columnRepository;

        }


        public static int ExtractAndConvertLastFour(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 3)
                return 0; // Return 0 if the input is null, empty, or has fewer than 3 characters

            int length = input.Length;
            string lastDigits;

            if (input[length - 1] == '0')
            {
                lastDigits = input.Substring(length - 4, 4); // Extract the last four characters
            }
            else
            {
                lastDigits = input.Substring(length - 4, 4); // Extract the last four characters
            }

            if (int.TryParse(lastDigits, out int result))
                return result;

            return 0;
        }

        public static int ExtractAndConvertSixFour(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input) || input.Length < 6)
                {
                    input = input.Substring(0, 6);
                    if (int.TryParse(input, out int resultc))
                    {
                        return resultc;
                    }
                    else
                    {
                        return 0;
                    }



                }
                else
                {
                    int length = input.Length;
                    string lastDigits;

                    if (input[length - 1] == '0')
                    {
                        lastDigits = input.Substring(0, 6); // Extract the last four characters
                    }
                    else
                    {
                        lastDigits = input.Substring(0, 6);// Extract the last four characters
                    }

                    if (int.TryParse(lastDigits, out int result))
                    {
                        return result;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            // Return 0 if the input is null, empty, or has fewer than 3 characters




        }



        private async Task<Data.ChartOfAccountManagementPosition> GetChartOfAccountManagementPosition(string accountNumber, List<ChartOfAccountManagementPosition> chartOfAccountManagementPositions)
        {
            Data.ChartOfAccountManagementPosition model = new ChartOfAccountManagementPosition();
            try
            {
                if (accountNumber.Length>7)
                {
                    var AN = ExtractAndConvertSixFour(accountNumber).ToString();
                    var PN = ExtractAndConvertLastFour(accountNumber).ToString();
                    var number = (AN.PadRight(6, '0')  + PN.PadRight(3, '0')+ "000");
                    List<ChartOfAccountManagementPosition> chartOfAccountManagementPositionxxx = chartOfAccountManagementPositions.Where(c => c.Old_AccountNumber.Trim() == accountNumber).ToList();
                    if (chartOfAccountManagementPositionxxx.Any())
                    {
                        if (accountNumber.Equals("340000005000"))
                        {
                            //340000001100

                            model = chartOfAccountManagementPositionxxx.FirstOrDefault();
                            model.PositionNumber =PN.Substring(0,PN.Length-1);
                                    
                        }
                        else if (accountNumber.Equals("340000001500"))
                        {
                            //340000001100   

                            model = chartOfAccountManagementPositionxxx.FirstOrDefault();
                            model.PositionNumber = PN.Substring(0, PN.Length - 1);

                        }
                        else if (accountNumber.Contains("451000"))
                        {
                            //340000001100  CMP9582166697

                            model = chartOfAccountManagementPositions.Find(x=>x.Id== "CMP9582166697");
                            model.PositionNumber = PN.Substring(0, PN.Length - 1);

                        }
                        else
                        {
                            model = chartOfAccountManagementPositionxxx.FirstOrDefault();
                        }
      


                    }
                    else
                    {
                        
                        var chartOfAccountManagementPositionxxxx0 = chartOfAccountManagementPositions.Where(c => c.Old_AccountNumber.Trim() == accountNumber);
                        if (chartOfAccountManagementPositionxxxx0.Any())
                        {
                            model = chartOfAccountManagementPositionxxxx0.FirstOrDefault();


                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    
                    var number1 = (accountNumber.PadRight(12, '0') );
                    var chartOfAccountManagementPositionxxx = chartOfAccountManagementPositions.Where(c => c.Old_AccountNumber.Trim() == accountNumber.Substring(0,6));
                    if (chartOfAccountManagementPositionxxx.Any())
                    {
                        model = chartOfAccountManagementPositionxxx.FirstOrDefault();


                    }
                    else
                    {
                        var chartOfAccountManagementPositionxxxx0 = chartOfAccountManagementPositions.Where(c => c.Old_AccountNumber.Trim() == accountNumber);
                        if (chartOfAccountManagementPositionxxxx0.Any())
                        {
                            model = chartOfAccountManagementPositionxxx.FirstOrDefault();


                        }
                        else
                        {

                        }
                    }
                }
         


                //var accountNumber1 = ExtractAndConvertSixFour(accountNumber).ToString();
                //var chartOfAccount = new Data.ChartOfAccount();
                //string tempNumber = string.Empty;
                //var chartOfAccountNumbers = _ChartOfAccountRepository.FindBy(c => c.TempData == accountNumber1);
                //var positionNumber = ExtractAndConvertLastFour(accountNumber).ToString();
                //positionNumber = positionNumber == "0" ? "0" : positionNumber.Substring(positionNumber.Length - 3);
                

 


                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private async Task<Data.ChartOfAccountManagementPosition> GetChartOfAccountManagementPositionxxx(string accountNumber)
        {
            try
            {
                var accountNumber1 = ExtractAndConvertSixFour(accountNumber).ToString();
                var chartOfAccount = new Data.ChartOfAccount();
                string tempNumber = string.Empty;
                var chartOfAccountNumbers = _ChartOfAccountRepository.FindBy(c => c.TempData == accountNumber1);
                var positionNumber = ExtractAndConvertLastFour(accountNumber).ToString();
                positionNumber = positionNumber == "0" ? "0" : positionNumber.Substring(positionNumber.Length - 3);
                if (chartOfAccountNumbers.Any())
                {
                    var liscount = chartOfAccountNumbers.Where(c => c.IsDebit == false).ToList();

                    chartOfAccount = chartOfAccountNumbers.First();
                    tempNumber = chartOfAccount.AccountNumber.PadRight(6, '0') + (positionNumber +"000" ).PadRight(6, '0');


                }
                else
                {

                }

                var positions = _chartOfAccountManagementPositionRepository.FindBy(c => c.TempData == tempNumber).ToList();


                return positions.FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private string GetLiaisonId(string branchCode, List<Branch> _branches, string branchCode1)
        {
            var branch = _branches.FirstOrDefault(x => x.branchCode == branchCode1);
            var branchModel = _branches.FirstOrDefault(x => x.branchCode == branchCode1);
            return branchModel?.id ?? "NO-LIAISONID";
        }
        private string GetLiaisonName(string branchOwner, List<Branch> _branches, string branchCode)
        {
            var branch = _branches.FirstOrDefault(x => x.branchCode == branchCode);
            var branchOwnerX = _branches.FirstOrDefault(x => x.branchCode == branchOwner);
            return branchOwnerX.name+ "-"+ branch.name +" Liaison Account";
        }

        public static List<string> ListOfIds()
        {

            return new List<string>
        {
            "COA7720628217",
            "COA0736764794",
            "COA9299689247",
            "COA9737877160",
            "COA0153440640",
            "COA2252849623",
            "COA3897852895",
            "COA5108288966",
            "COA6548526555",
            "COA1665061009",
            "COA5781761914",
            "CA472999241604",
            "COA9762783150",
            "COA9275179335",
            "COA1909389677"
        };


        }
        private async Task<Data.Account> CreateAccount(List<Branch> _branches,  AccountModelX item, Branch branch, PositionChart chartModel, Data.ChartOfAccountManagementPosition chartOfAccountManagementPosition)
        {
            Data.Account accountEntity = new Data.Account();
            var chartOfAccount = await _ChartOfAccountRepository.FindAsync(chartModel.ChartOfAccountManagementPosition.ChartOfAccountId);
            if (item.AccountNumber.Contains("451000"))
            {
                  chartOfAccount = await _ChartOfAccountRepository.FindAsync("CMP9582166697");

            }

            if (chartModel.Is451peresent)
            {
                var accountEntry = await CreateAccountEntryAsync(item, branch.id, branch.branchCode, chartModel.ChartOfAccountManagementPosition, chartOfAccount);
                  accountEntity = _mapper.Map<Data.Account>(accountEntry);
                accountEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "AC");
                string branchCode = item.AccountNumber.Substring(item.AccountNumber.Length - 3);
                accountEntity.AccountNumberCU = accountEntry.AccountNumber.PadRight(6,'0') + branch.branchCode + branchCode;
                var liaisonId = GetLiaisonId(branch.branchCode, _branches, branchCode);
               var accountName= GetLiaisonName(branch.branchCode, _branches, branchCode);
                accountEntity.SetAccountEntityBranchCode(accountStatus: AccountMappingStatusEnum.MappedToSystem,
                    entity:accountEntity,
                 accountNumber:accountEntry.AccountNumber,
                    accountName: accountName,
                   chartOfAccountId: chartOfAccountManagementPosition.Id,
                  accountOwnerId:  branch.id,
                  bankCOde:  _pathHelper.BankManagement_BankCode,
                   branchCode: branch.branchCode,
                   accountNumberManagementPosition: branchCode,
                  beginningBalance:  Math.Abs(item.BeginningCreditBalance - item.BeginningDebitBalance),
                  beginningBalanceDebit:  item.BeginningDebitBalance,
                  beginningBalanceCredit:  item.BeginningCreditBalance,
                   movementDebit:  item.MovementDebitBalance,
                  movementCredit:  item.MovementCreditBalance,
                  currentBalance: Math.Abs(item.EndBalanceCredit - item.EndBalanceDebit), // GetEndingBalance(item),
                 bookingDirection:   item.BookingDirection,
                liaisonId:    liaisonId
                );
            }
            else
            {
                var accountEntry = await CreateAccountEntryAsync(item, branch.id, branch.branchCode, chartModel.ChartOfAccountManagementPosition, chartOfAccount);
                  accountEntity = _mapper.Map<Data.Account>(accountEntry);
                accountEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "AC");
                accountEntity.SetAccountEntityMG(AccountMappingStatusEnum.MappedToSystem,
                    accountEntity,
                    accountEntry.AccountNumber,
                    accountEntry.AccountName,
                    chartOfAccountManagementPosition.Id,
                    branch.id,
                  _pathHelper.BankManagement_BankCode,
                    accountEntry.OwnerBranchCode,
                    chartOfAccountManagementPosition.PositionNumber,
                    Math.Abs(item.BeginningCreditBalance - item.BeginningDebitBalance),
                    item.BeginningDebitBalance,
                    item.BeginningCreditBalance,

                     item.MovementDebitBalance,
                    item.MovementCreditBalance,
                   /* GetEndingBalance(item)*/ Math.Abs(item.EndBalanceCredit - item.EndBalanceDebit),
                    item.BookingDirection,
                    "NO-LIAISONID"
                );
            }

            return accountEntity;
        }

        private bool IsdebitNormal(string? accountNumber)
        {
            // Determine account behavior based on OHADA rules
            return accountNumber.StartsWith("2") || // Fixed Assets
                                                    //   || // Inventory
                               accountNumber.StartsWith("5") || // Financial
                                accountNumber.StartsWith("6");   // Expenses


        }
        private bool IsCreditNormal(string? accountNumber)
        {
            // Determine account behavior based on OHADA rules
            return accountNumber.StartsWith("1") || // Fixed Assets
                                                    //   || // Inventory
                               accountNumber.StartsWith("7") || // Financial
                                accountNumber.StartsWith("3");   // Expenses


        }
        private decimal GetEndingBalance(AccountModelX item)
        {
            decimal value = 0;
            if ((item.EndBalanceCredit > item.EndBalanceDebit) && (item.AccountNumber.StartsWith("1") || item.AccountNumber.StartsWith("7")))
            {
                value = item.EndBalanceCredit;

            }
            else if ((item.EndBalanceDebit > item.EndBalanceCredit) && (item.AccountNumber.StartsWith("1") || item.AccountNumber.StartsWith("7")))
            {
                value = item.EndBalanceCredit * -1;
            }
            else if ((item.EndBalanceDebit > item.EndBalanceCredit) && (item.AccountNumber.StartsWith("2") || item.AccountNumber.StartsWith("5") || item.AccountNumber.StartsWith("6")))
            {
                value = item.EndBalanceDebit;
            }
            else if ((item.EndBalanceCredit > item.EndBalanceDebit) && (item.AccountNumber.StartsWith("2") || item.AccountNumber.StartsWith("5") || item.AccountNumber.StartsWith("6")))
            {
                value = item.EndBalanceCredit * -1;
            }
            else if (item.AccountNumber.StartsWith("4") || item.AccountNumber.StartsWith("3") || item.AccountNumber.StartsWith("8") || item.AccountNumber.StartsWith("9"))
            {
                value = (item.EndBalanceCredit > 0) ? item.EndBalanceCredit : item.EndBalanceDebit;
            }
            //if (IsdebitNormal(item.AccountNumber))
            //{
            //    value = value - item.BeginningDebitBalance+ item.BeginningCreditBalance ;
            //}
            //else
            //{
            //    value = value - item.BeginningCreditBalance + item.BeginningDebitBalance;
            //}
            return value;
        }
        public async Task<ServiceResponse<UploadAccountResult>> Handle(UploadAccount request, CancellationToken cancellationToken)
        {
            var listOfChartOfAccountManagementPositions = _chartOfAccountManagementPositionRepository.All.ToList();
            var result = new UploadAccountResult();
            var accountsNotPresent = new List<AccountModelX>();
            var accounts = new List<Data.Account>();
            var accountListExs = new List<Data.Account>();
            var accountingEntries = new List<Data.AccountingEntry>();
            var accounnewAccountingEntriesListExs = new List<Data.AccountingEntry>();
            string errorMessage = "";
            DateTime dateTime =(await APICallHelper.GetAccountingDateOpen(_pathHelper, _userInfoToken, request.BranchId)).Data;

            List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
            try
            {
                var branch = branches.Find(x=>x.id==request.BranchId);

                
                var items = _accountRepository.FindBy(xx => xx.BranchCode == branch.branchCode);
                if (items.Any())
                {

                    errorMessage = $"You have already uploaded the trial balance of {branch.name} to re-upload contact system administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Read, LogLevelInfo.Warning);
                }

             
                foreach (var item in request.AccountModelList)
                {
                    var chartOfAccountManagementPosition = await GetChartOfAccountManagementPosition(item.AccountNumber,listOfChartOfAccountManagementPositions);               
                    if (chartOfAccountManagementPosition == null)
                    {
                        string number = "";
                        var AN = ExtractAndConvertSixFour(item.AccountNumber).ToString();
                        var PN = ExtractAndConvertLastFour(item.AccountNumber).ToString();
                        number = (AN.PadRight(6, '0') + PN.PadRight(3, '0') + "000");
                        var chartOfAccountManagementPositionxxx = listOfChartOfAccountManagementPositions.Where(c => c.Old_AccountNumber.Trim() == number);
                        if (!chartOfAccountManagementPositionxxx.Any())
                        {

                            accountsNotPresent.Add(item);
                            continue;
                        }
                        chartOfAccountManagementPosition = chartOfAccountManagementPositionxxx.FirstOrDefault();
                    }
                    else
                    {
                        var chartModel = await CheckIfChartOfAccountIsCreatedByIdAsync(item, chartOfAccountManagementPosition.Id, listOfChartOfAccountManagementPositions);
                        if (!chartModel.Isperesent)
                        {
                            if (item.AccountNumber.Contains("451000"))
                            {
                                var model = listOfChartOfAccountManagementPositions.Where(c => c.Old_AccountNumber.Trim() == item.AccountNumber);
                                chartModel.Is451peresent = true;
                                chartModel.chartOfAccountID = model.Count() == 1 ? model.FirstOrDefault().ChartOfAccountId : throw new Exception("There is more than one account with the accountnumber" + item.AccountNumber);
                                chartModel.ChartOfAccountManagementPosition = model.Count() == 1 ? model.FirstOrDefault() : throw new Exception("There is more than one account with the accountnumber" + item.AccountNumber);
                                chartModel.Is18112peresent = false;
                                chartModel.ChartOfAccountManagementPositionId = model.Count() == 1 ? model.FirstOrDefault().Id : throw new Exception("There is more than one account with the accountnumber" + item.AccountNumber);
                                chartModel.Isperesent = true;
                            }
                            else
                            {
                                accountsNotPresent.Add(item);
                                continue;
                            }

                        }

                        var account = await CreateAccount(branches, item, branch, chartModel, chartOfAccountManagementPosition);
                    
                        var entry = CreateAccountingEntry(item,account, await GetChartOfAccount(item, chartOfAccountManagementPosition.ChartOfAccountId),dateTime);

                        accounts.Add(account);
                        accountingEntries.Add(entry);

                    }

                }
                if (accounts.Where(x => x.AccountNumber.StartsWith("57102")).Count() > 1)
                {
                    var newAccountList = accounts.Where(x => x.AccountNumber.StartsWith("57102"));
                    accountListExs = newAccountList.ToList();
                    var numberOfItems = accounts.RemoveAll(x => newAccountList.Contains(x));
                    accounts.Add(CreateNewCashInHand(accountListExs));
                }
                if (accountingEntries.Where(x => x.AccountNumber.StartsWith("57102")).Count() > 1)
                {
                    var newAccountingEntriesList = accountingEntries.Where(x => x.AccountNumber.StartsWith("57102"));
                    accounnewAccountingEntriesListExs = newAccountingEntriesList.ToList();
                    var numberOfItems = accountingEntries.RemoveAll(x => newAccountingEntriesList.Contains(x));
                    accountingEntries.Add(CreateCombineEntry(accounnewAccountingEntriesListExs));
                }
                var modellist = await SaveAccounts(accounts, accountingEntries, request.BranchId, accountsNotPresent, request.IsHarmonizationActivated,dateTime);
                 TrialBalanceUploadResult trial = request.IsHarmonizationActivated? new TrialBalanceUploadResult { AccountNotPresent = accountsNotPresent, OriginalFile = request.AccountModelList }: new TrialBalanceUploadResult { AccountNotPresent = new List<AccountModelX>(), OriginalFile = request.AccountModelList };


                result = await CreateUploadResult(request.IsHarmonizationActivated,trial, request.AccountModelList, branch);
            }
            catch (Exception ex)
            {
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error);
                return ServiceResponse<UploadAccountResult>.Return500(ex.Message);
            }

            return ServiceResponse<UploadAccountResult>.ReturnResultWith200(result);
        }

        private Data.AccountingEntry CreateCombineEntry(List<Data.AccountingEntry> accounnewAccountingEntriesListExs)
        {
            decimal movDr = accounnewAccountingEntriesListExs.Sum(x => x.DrAmount);
            decimal movCr = accounnewAccountingEntriesListExs.Sum(x => x.CrAmount);
            decimal CurrentBalance = accounnewAccountingEntriesListExs.Sum(x => x.CurrentBalance);
            var model = accounnewAccountingEntriesListExs.ToArray()[0];
            model.DrAmount = movDr;
            model.CrAmount = movCr;
            model.CurrentBalance = CurrentBalance;
            


            return model;
        }

        private Data.Account CreateNewCashInHand(IEnumerable<Data.Account> newAccountList)
        {
          
            decimal beginningBalanceDr = newAccountList.Sum(x=>x.BeginningBalanceDebit);
            decimal beginningBalanceCr = newAccountList.Sum(x => x.BeginningBalanceCredit);
            decimal beginningBalance = newAccountList.Sum(x => x.BeginningBalance);
            decimal debitBalance = newAccountList.Sum(x => x.DebitBalance);
            decimal creditBalance = newAccountList.Sum(x => x.CreditBalance);
            decimal currentBalance = newAccountList.Sum(x => x.CurrentBalance);
            var model = newAccountList.ToArray()[0];
             model.BeginningBalanceDebit = beginningBalanceDr;
            model.BeginningBalanceCredit = beginningBalanceCr;
            model.BeginningBalance = beginningBalance;
            model.DebitBalance = debitBalance;
            model.CreditBalance = creditBalance;
            model.CurrentBalance = currentBalance;
            return model;
        }

        private async Task<UploadAccountResult> CreateUploadResult(bool IsHarmonizationActivated,TrialBalanceUploadResult model, List<AccountModelX> allAccounts, Branch branch)
        {
            var result = new UploadAccountResult
            {
                Account_Present = model.AccountNotPresent.Count,
                Total_Account = allAccounts.Count,
                UploadStatus = model.AccountNotPresent.Count == 0,
                BranchName = branch.name
            };
            var trailBalanceUpload = new TrailBalanceUploud
            {
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "FU"),
                UploadStatus = true,
                BranchName = branch.name,
                Account_Left = result.Account_Present.ToString(),
                UserName = _userInfoToken.FullName
            };

            _trailBalanceUploudRepository.Add(trailBalanceUpload);
            var auxlist = model.AccountNotPresent.Count() == 0 ? GetMFI_ChartOfAccount(branch.id) : model.AccountNotPresent;
            var command = new AddDocumentUploadedCommand
            {
                FormFiles = _fileProcessor.PrepareFileOnServer(IsHarmonizationActivated, model.AccountNotPresent.Count() == 0, model, ".xlsx", (model.AccountNotPresent.Count == 0 ? $"{branch.name.Trim()}_TrialBalance": $"{branch.name.Trim()}AccountNotPresent")),
                IsSynchronus = true,
                OperationID = trailBalanceUpload.Id,
                DocumentType = model.AccountNotPresent.Count == 0 ? "ALPHA TRIAL BALANCE VALIDATED" : "ALPHA TRIAL BALANCE",
                ServiceType = "AccountingManagementService",
                DocumentId = "N/A",
                CallBackBaseUrl = "N/A",
                CallBackEndPoint = "N/A",
                RemoteFilePath = model.AccountNotPresent.Count == 0
                    ? $"{branch.branchCode}/{_fileProcessor._pathHelper.FileUpload_MigrationValidatedPath}"
                    : $"{_fileProcessor._pathHelper.FileUpload_MigrationPath}"
            };

            var dtoModel = await APICallHelper.UploadExcelDocument(command, _pathHelper.FileUploadEndpointURL, _userInfoToken.Token);

            trailBalanceUpload.FilePath = dtoModel.FullPath;
            result.file_path = dtoModel.FullPath;

            await _uow.SaveAsync();

            return result;
        }

        private List<AccountModelX> GetMFI_ChartOfAccount(string id)
        {
            List<AccountModelX> listModel = new List<AccountModelX>();
            var list = _accountRepository.FindBy(Bra=>Bra.BranchId== id);
            var modelList = _mapper.Map<List<AccountDto>>(list.ToList());

            foreach (var item in modelList)
            {
                listModel.Add(AccountModelX.ConvertToAccountModelX(item));
            }
            return  listModel;
        }

        private async Task<(List<Data.Account>, List<Data.AccountingEntry>)> SaveAccounts(List<Data.Account> accounts, List<Data.AccountingEntry> accountingEntries, string branchId, List<AccountModelX> accountsNotPresent,bool IsHarmonisationActivated,DateTime dateTime)
        {
            try
            {

                if (accountsNotPresent.Count() == 0)
                {
                    if (accounts.Any())
                    {
                        _accountRepository.AddRange(accounts);
                        _accountingEntryRepository.AddRange(accountingEntries);

                        _userInfoToken.BranchId = branchId;
                        await _uow.SavingMigrationAsync(branchId);

                        _logger.LogInformation($"Saved {accounts.Count} accounts and {accountingEntries.Count} accounting entries for branch {branchId}");
                    }
                    else
                    {
                        _logger.LogInformation($"No accounts to save for branch {branchId}");
                    }
                }
                else 
                {
                    if (IsHarmonisationActivated)
                    {

                    }
                    else
                    {
                        List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                        var models = await CreatDefaultAccountAsync(accountsNotPresent, branches.Find(x => x.id.Equals(branchId)), dateTime);
                        accounts.AddRange(models.Item1);
                        _accountRepository.AddRange(accounts);
                        accountingEntries.AddRange(models.Item2);
                        _accountingEntryRepository.AddRange(accountingEntries);

                        _userInfoToken.BranchId = branchId;
                        await _uow.SavingMigrationAsync(branchId);

                        _logger.LogInformation($"Saved {accounts.Count} accounts and {accountingEntries.Count} accounting entries for branch {branchId}");

                    }


                }
                return (accounts, accountingEntries);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving accounts for branch {branchId}");
                throw; // Re-throw the exception to be handled by the calling method
            }
        }

 

        private async Task<(List<Data.Account>,List<Data.AccountingEntry>)> CreatDefaultAccountAsync( List<AccountModelX> ListOfAccountModelX, Branch branch, DateTime  dateTime)
        {
             List<Data.Account> listOfAccounts = new List<Data.Account>();
            List<Data.AccountingEntry> listOfAccountingEntries = new List<Data.AccountingEntry>();
            var chartOfAccount = await  _ChartOfAccountRepository.FindBy(x=>x.ParentAccountNumber=="ROOT").FirstOrDefaultAsync();
            var mfi_Chart = await _chartOfAccountManagementPositionRepository.All.Where(x=>x.ChartOfAccountId== chartOfAccount.Id).FirstOrDefaultAsync();
            foreach (var item in ListOfAccountModelX)
            {
                var accountEntry = await CreateAccountEntryAsync( item, branch.id, branch.branchCode, mfi_Chart, chartOfAccount);
                var accountEntity = _mapper.Map<Data.Account>(accountEntry);
                accountEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "AC");
                accountEntity.SetAccountEntityMG(AccountMappingStatusEnum.NotMappedToSystem,
                    accountEntity,
                    accountEntry.AccountNumber,
                    accountEntry.AccountName,
                    mfi_Chart.Id,
                    branch.id,
                  _pathHelper.BankManagement_BankCode,
                    accountEntry.OwnerBranchCode,
                    mfi_Chart.PositionNumber,
                    Math.Abs(item.BeginningCreditBalance - item.BeginningDebitBalance),
                    item.BeginningDebitBalance,
                    item.BeginningCreditBalance,
                    item.MovementDebitBalance,
                    item.MovementCreditBalance,
                    GetEndingBalance(item),
                    item.BookingDirection,
                    "NO-LIAISONID"
                );
                listOfAccounts.Add( accountEntity );
                listOfAccountingEntries.Add( CreateAccountingEntry(item,accountEntity, chartOfAccount, dateTime));
            }
 
         
           

            return (listOfAccounts, listOfAccountingEntries);
        }

        private async Task<Data.ChartOfAccount> GetChartOfAccount(AccountModelX item, string chartOfAccountId)
        {
            var chartOfAccount = await _ChartOfAccountRepository.FindAsync(chartOfAccountId);

            if (chartOfAccount == null)
            {
                var schartOfAccount = _chartOfAccountManagementPositionRepository.FindBy(pp => pp.AccountNumber == item.ChartofAccount);
                if (schartOfAccount.Any())
                {
                    chartOfAccount = await _ChartOfAccountRepository.FindAsync(schartOfAccount.FirstOrDefault().ChartOfAccountId);
                    

                }
                else
                {
                    return null;
                }
            }

            return chartOfAccount;
        }
       

        private  async Task<AddAccountCommand> CreateAccountEntryAsync( AccountModelX item, string branchId, string BranchCode, Data.ChartOfAccountManagementPosition model, Data.ChartOfAccount chartOfAccount)
        {
             
            if (item.AccountNumber.Contains("451000"))
            {
                chartOfAccount = await _ChartOfAccountRepository.FindAsync("CA083008603577");

            }

            return new AddAccountCommand
                {
                    AccountNumberManagementPosition = model.PositionNumber,
                    AccountOwnerId = BranchId,
                    AccountName = item.AccountName,
                    OwnerBranchCode = BranchCode,
                    AccountNumber = chartOfAccount.AccountNumber,
                    AccountNumberNetwok = (chartOfAccount.AccountNumber.PadRight(6, '0') + model.PositionNumber + _pathHelper.BankManagement_BankCode + BranchCode).PadLeft(15, '0') ,
                    AccountNumberCU = chartOfAccount.AccountNumber.PadRight(6, '0') + model.PositionNumber.PadLeft(3, '0') + BranchCode.PadLeft(3, '0'),
                    ChartOfAccountManagementPositionId = model.Id,
                    AccountTypeId = BranchId,
                    AccountCategoryId = chartOfAccount.AccountCartegoryId
                };
           
      
        }
        private Data.AccountingEntry CreateAccountingEntry(AccountModelX item, Data.Account AccountEntity, Data.ChartOfAccount chartOfAccount, DateTime dateTime)
        {
            return new Data.AccountingEntry
            {
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "MG"),
                AccountId = AccountEntity.Id,
                EventCode = "MIGRATION",
                DrAmount = item.MovementDebitBalance,// IsdebitNormal(item.AccountNumber) ? item.EndBalanceDebit : 0,
                CurrentBalance = AccountEntity.CurrentBalance,
                CrAmount = item.MovementCreditBalance,//IsCreditNormal(item.AccountNumber) ? item.EndBalanceCredit : 0,
                ExternalBranchId = AccountEntity.AccountOwnerId,
                IsAuxilaryEntry = false,
                BankId = "",
                BranchId = AccountEntity.AccountOwnerId,
                ReferenceID = BaseUtilities.GenerateInsuranceUniqueNumber(9, "MG"),
                EntryDate = dateTime,
                EntryType = GetEntryType(AccountEntity),//AccountOperationType.CREDIT.ToString(),
                CrAccountId =  AccountEntity.Id ,
                DrAccountId =  AccountEntity.Id ,
                Representative = "MIGRATION",
                CrAccountNumber =  AccountEntity.AccountNumberCU,
                DrAccountNumber = AccountEntity.AccountNumberCU,
                AccountNumber = chartOfAccount.AccountNumber,
                Amount = AccountEntity.CurrentBalance,
                CrCurrentBalance = (item.EndBalanceCredit > item.EndBalanceDebit) ? AccountEntity.CurrentBalance : 0,
                DrCurrentBalance = (item.EndBalanceDebit > item.EndBalanceCredit) ? AccountEntity.CurrentBalance : 0,
                
                DrBalanceBroughtForward =  item.EndBalanceDebit ,
                CrBalanceBroughtForward = item.EndBalanceCredit,
                Status = PostingStatus.Posted.ToString(),
                ValueDate = dateTime,
                Source = "SYSTEM",
                
                InitiatorId = "NOT SET",
                Naration = "Balance brought forward",

                OperationType = "NOT SET",
                Currency = Currency.GetCurrency(),
                AccountCartegory = chartOfAccount.AccountCartegoryId,
                AccountNumberReference = AccountEntity.AccountNumberReference,

            };
        }

        private string GetEntryType(Data.Account accountEntity)
        {
            return accountEntity.DebitBalance == 0 ? AccountOperationType.DEBIT.ToString() : AccountOperationType.CREDIT.ToString();
        }







        private async Task<PositionChart> CheckIfChartOfAccountIsCreatedByIdAsync(AccountModelX model, string chartOfAccountId,List<ChartOfAccountManagementPosition> listOfChartOfAccountManagementPositions)
        {
            PositionChart positionChart = new PositionChart();
            //Check if chart of account name is in modelList
            if (model.AccountNumber.Contains("45100000"))
            {
                var list = listOfChartOfAccountManagementPositions.Where(c=>c.AccountNumber== "451000");
                if (list.Any())
                {

                    var mopel = list.FirstOrDefault();
                    positionChart.Isperesent = true;
                    positionChart.Is451peresent = true;
                    positionChart.chartOfAccountID = mopel.ChartOfAccountId;
                    positionChart.ChartOfAccountManagementPositionId = mopel.Id;
                    positionChart.ChartOfAccountManagementPosition = mopel;
                }
                else
                {
                    positionChart.Isperesent = false;
                    positionChart.Is451peresent = false;
                }

            }
            else
            {

                var list =  listOfChartOfAccountManagementPositions.Find(x=>x.Id.Equals(chartOfAccountId));
                if (list != null)
                {


                    positionChart.Isperesent = true;
                    positionChart.chartOfAccountID = list.ChartOfAccountId;
                    positionChart.ChartOfAccountManagementPositionId = list.Id;
                    positionChart.ChartOfAccountManagementPosition = list;
                }
                else
                {
                    positionChart.Isperesent = false;
                    positionChart.Is451peresent = false;
                }


            }

            return positionChart;

        }
      }

    internal class PositionChart
    {
        internal string chartOfAccountID;

        public bool Isperesent { get; set; }
        public bool Is451peresent { get; set; }
        public string ChartOfAccountManagementPositionId { get; internal set; }
        public ChartOfAccountManagementPosition ChartOfAccountManagementPosition { get; internal set; }
        public bool Is18112peresent { get; internal set; }
    }
}
