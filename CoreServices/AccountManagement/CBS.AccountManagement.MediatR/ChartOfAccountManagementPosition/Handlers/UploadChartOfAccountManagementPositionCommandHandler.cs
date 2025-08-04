
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
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.EntityFrameworkCore;
using CBS.AccountManagement.MediatR.ChartOfAccount;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System;


namespace CBS.AccountManagement.MediatR.Handlers
{

    public class UploadChartOfAccountManagementPositionCommandHandler : IRequestHandler<UploadChartOfAccountManagementPositionCommand, ServiceResponse<bool>>
    {
        // Dependencies
        private readonly IAccountRepository _accountRepository;

        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly ILogger<UploadAccountCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _madiator;
        private readonly PathHelper _pathHelper;
        private readonly IUnitOfWork<POSContext> _uow;
        public ChartOfAccountService _chartOfAccountService { get; set; }

        public string BranchId { get; private set; }

        // Constructor to inject dependencies
        public UploadChartOfAccountManagementPositionCommandHandler(IAccountRepository accountRepository, ILogger<UploadAccountCommandHandler> logger, IMapper mapper, UserInfoToken userInfoToken, IChartOfAccountRepository? chartOfAccountRepository, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository, IUnitOfWork<POSContext>? uow, IMediator? madiator, PathHelper? pathHelper, IAccountingEntryRepository? accountingEntryRepository)
        {
            _ChartOfAccountRepository = chartOfAccountRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _chartOfAccountService = new ChartOfAccountService();
            _logger = logger;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _madiator = madiator;
            _pathHelper = pathHelper;

        }

        public static string ExtractChartOfAccountNumber(string word)
        {
            if (string.IsNullOrEmpty(word) || word.Length < 6)
            {
                return word;
            }

            return word.Substring(0, 6);
        }
        public static int ExtractManagementPositions(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 3)
                return 0; // Return 0 if the input is null, empty, or has fewer than 3 characters

            int length = input.Length;
            string lastDigits = "0";

            if (input[length - 1] == '0')
            {
                lastDigits = input.Substring(length - 3, 3); // Extract the last three characters
            }


            if (int.TryParse(lastDigits, out int result))
                return result; // If the conversion is successful, return the converted value

            return 0; // If the conversion fails, return 0
        }
        public async Task<Data.ChartOfAccount> CreateAccountAsync(string accountNumber, string labelFr, string labelEn, bool isBalanceAccount)
        {
            // Determine the parent account number (all but the last character of the account number)
            string parentAccountNumber = accountNumber.Length > 1 ? accountNumber.Substring(0, accountNumber.Length - 1) : null;

            Data.ChartOfAccount parentAccount = null;
            if (!string.IsNullOrEmpty(parentAccountNumber))
            {
                // Try to find the parent account
                var parentAccounts = _ChartOfAccountRepository.FindBy(a => a.AccountNumber == parentAccountNumber);

                // If parent account doesn't exist, create it recursively
                if (parentAccounts.Any())
                {
                    parentAccount = await CreateAccountAsync(
                        accountNumber: parentAccountNumber,
                        labelFr: parentAccounts.FirstOrDefault().LabelFr,
                        labelEn: parentAccounts.FirstOrDefault().LabelEn,
                        isBalanceAccount: isBalanceAccount
                    );
                }
            }

            var newAccount = new Data.ChartOfAccount
            {
                Id = Guid.NewGuid().ToString(),
                AccountNumber = accountNumber,
                LabelFr = labelFr,
                LabelEn = labelEn,
                IsBalanceAccount = isBalanceAccount,
                ParentAccountNumber = parentAccountNumber,
                ParentAccountId = parentAccount?.Id,
                AccountCartegoryId = parentAccount?.AccountCartegoryId,
            };

            try
            {
                _ChartOfAccountRepository.Add(newAccount);
                await _uow.SaveAsync();
                return newAccount;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> Handle(UploadChartOfAccountManagementPositionCommand request, CancellationToken cancellationToken)
        {
            List<Data.ChartOfAccountManagementPosition> modelList = new List<Data.ChartOfAccountManagementPosition>();
            List<Data.Account> accounts = new List<Data.Account>();
            List<Data.AccountingEntry> accountingEntries = new List<Data.AccountingEntry>();
            Data.ChartOfAccount chartOfAccount = new Data.ChartOfAccount();
            List<Data.ChartOfAccount> chartOfAccounts = new List<Data.ChartOfAccount>();
            List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
            List<ChartOfAccountManagementPositionFile> accountsNotPresent = new List<ChartOfAccountManagementPositionFile>();
            List<Data.ChartOfAccount> ListOfchartOfAccounts = new List<Data.ChartOfAccount>();
            Data.ChartOfAccountManagementPosition model = new Data.ChartOfAccountManagementPosition();
            Data.ChartOfAccountManagementPosition modelEx = new Data.ChartOfAccountManagementPosition();
            bool modelEx451Ispresent = false;
            try
            {
                foreach (var item in request.ChartOfAccountManagementPositionFile)
                {
               
                    var positionNumber = ExtractManagementPositions(item.AccountNumber.Substring(6)).ToString();
                    var AccountNumber = ExtractChartOfAccountNumber(item.AccountNumber).Substring(0,6);
                    chartOfAccounts = _ChartOfAccountRepository.FindBy(c => c.TempData == AccountNumber).ToList();
                    if (chartOfAccounts.Any())
                    {
                        if (item.AccountNumber.Contains("740101"))
                        {
                          
                        }
                        if (item.AccountNumber.Contains("340000"))
                        {
                            if (item.AccountNumber.Equals("340000005000"))
                            {
                                //340000001100


                            }

                        }
                        chartOfAccount = chartOfAccounts.FirstOrDefault();
                        if (item.AccountNumber.Contains("451000"))
                        {
                            if (item.AccountNumber.Contains("451000")|| item.AccountNumber.Contains("451") && modelEx451Ispresent == false)
                            {
                                modelEx451Ispresent = true;
                                model = CreateChartOfAccountManagementPosition(chartOfAccount, item,  item.AccountNumber.Substring(0, 6), modelList, item.AccountNumberBapCCUL, item.IsUniqueToBranch);
                                modelList.Add(model);
                            }
                            else if (item.AccountNumber.Contains("45010"))
                            {
                                model = CreateChartOfAccountManagementPosition(chartOfAccount, item , item.AccountNumber.Substring(0, 6), modelList, item.AccountNumberBapCCUL, item.IsUniqueToBranch);
                                modelList.Add(model);

                            }


                        }
                        else
                        {
                            if (item.AccountNumber.Equals("34000000110")|| item.AccountNumber.Contains("340000005000"))
                            {//340000001100 //340000001100



                            }

                            model = CreateChartOfAccountManagementPositionfix(chartOfAccount, item,positionNumber, modelList, item.AccountNumber.Substring(0, 6), item.IsUniqueToBranch);
                            modelList.Add(model);

                        }
                    }
                    else
                    {
                        var extractValue = ExtractTrailingZeros(Convert.ToInt32(AccountNumber));
                        chartOfAccounts = _ChartOfAccountRepository.FindBy(c => c.AccountNumber == extractValue.ToString()).ToList();
                        if (chartOfAccounts.Any()) 
                        {
                            model = CreateChartOfAccountManagementPosition(chartOfAccount, item, item.AccountNumber.Substring(0, 6), modelList, item.AccountNumberBapCCUL, item.IsUniqueToBranch);
                            modelList.Add(model);
                        }
                        else
                        {
                            chartOfAccounts = _ChartOfAccountRepository.FindBy(c => c.TempData == item.AccountNumber).ToList();
                            if (chartOfAccounts.Any())
                            {
                                model = CreateChartOfAccountManagementPosition(chartOfAccount, item, item.AccountNumber.Substring(0, 6), modelList , item.AccountNumberBapCCUL,item.IsUniqueToBranch);
                                modelList.Add(model);
                            }
                            else
                            {
                                accountsNotPresent.Add(item);
                                List<Data.ChartOfAccount> list = await _chartOfAccountService.CreateChartOfAccountsHierarchy(item.AccountNumber.Substring(0,6));
                                if (list == null)
                                {

                                }
                                else
                                {
                                    ListOfchartOfAccounts.AddRange(list);
                                }

                                //var existingAccount = await _ChartOfAccountRepository.All.FirstOrDefaultAsync(a => a.AccountNumber == item.AccountNumber);
                                //modelList.Add(model);

                            }

                        }


                    }
                }
                if (ListOfchartOfAccounts.Count() == 0)
                {

                }
                else
                {
                    UploadChartOfAccountQuery ChartOfAccountQuery = new UploadChartOfAccountQuery();
                    ChartOfAccountQuery.ChartOfAccounts = new List<ChartOfAccountDto>();
                    ChartOfAccountQuery.ChartOfAccounts.AddRange(_mapper.Map<List<ChartOfAccountDto>>(ListOfchartOfAccounts));
                    var resisult = _madiator.Send(ChartOfAccountQuery, cancellationToken).GetAwaiter().GetResult();
                    modelList.AddRange(RebuildChartOfAccountManagementPositionFromBapCCULChart(accountsNotPresent, resisult.Data));
                }



                _chartOfAccountManagementPositionRepository.AddRange(modelList);

                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        private List<ChartOfAccountManagementPosition> RebuildChartOfAccountManagementPositionFromBapCCULChart(
     List<ChartOfAccountManagementPositionFile> accountsNotPresent,
    List<ChartOfAccountDto> result)
        {
            List<ChartOfAccountManagementPosition> rebuiltAccounts = new List<ChartOfAccountManagementPosition>();


            foreach (var item in accountsNotPresent)
            {
                var matchingChartAccount = _ChartOfAccountRepository.All.FirstOrDefault(c => c.AccountNumber == item.AccountNumber.Substring(0,6));

                if (matchingChartAccount != null)
                {
                    var rebuiltAccount = RebuildChartOfAccountManagementPosition(matchingChartAccount, item);

                    rebuiltAccounts.Add(rebuiltAccount);
                }
                else
                {


                }
            }

            return rebuiltAccounts;
        }
        private Data.ChartOfAccountManagementPosition RebuildChartOfAccountManagementPosition(Data.ChartOfAccount chartOfAccount, ChartOfAccountManagementPositionFile item)
        {


            return new Data.ChartOfAccountManagementPosition
            {
                AccountNumber = item.AccountNumber,
                Old_AccountNumber = item.AccountNumber,
                ChartOfAccountId = chartOfAccount.Id,
                PositionNumber = "000", // (positionNumber.ToString().Length == 4) ? positionNumber.ToString().Substring(0, 3).PadLeft(3, '0') : positionNumber.ToString().PadLeft(3, '0'),
                Description = item.AccountName,
                RootDescription = chartOfAccount.LabelEn,
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "CMP")
            };
        }
        public static int ExtractTrailingZeros(int num)
        {
            int count = 0;
            int temp = num;

            // Remove leading zeros
            while (temp % 10 == 0)
            {
                count++;
                temp /= 10;
            }

            return temp;
        }
        private UploadChartOfAccountQuery CreateChartOfAccountQueryObject(List<ChartOfAccountManagementPositionFile> accountsNotPresent)
        {
            List<ChartOfAccountDto> chartOfAccountDtos = new List<ChartOfAccountDto>();
            foreach (var item in accountsNotPresent)
            {

                chartOfAccountDtos.AddRange(CreateChartOfAccountDto(item));
            }

            return new UploadChartOfAccountQuery
            {
                ChartOfAccounts = chartOfAccountDtos
            };

        }

        private List<ChartOfAccountDto> CreateChartOfAccountDto(ChartOfAccountManagementPositionFile item)
        {
            List<ChartOfAccountDto> modelList = new List<ChartOfAccountDto>();
            var AccountNumber = ExtractChartOfAccountNumber(item.AccountNumber).ToString();
            int i = 0;

            foreach (var item1 in AccountNumber)
            {
                var model = _ChartOfAccountRepository.FindBy(p => p.AccountNumber == AccountNumber.Substring(i, 6));
                if (!model.Any())
                {
                    modelList.Add(CreateChartOfAccountDtomodel(AccountNumber.Substring(i, 6), AccountNumber.Substring(i - 1, 6)));
                }
                i++;
            }
            return modelList;
        }

        private Data.ChartOfAccountDto CreateChartOfAccountDtomodel(string AccountNumber, string rootAccountNumber)
        {
            Data.ChartOfAccountDto chart = new Data.ChartOfAccountDto();
            var models = _ChartOfAccountRepository.FindBy(p => p.AccountNumber == rootAccountNumber);
            if (!models.Any())
            {
                var model = models.FirstOrDefault();
                chart = new Data.ChartOfAccountDto
                {
                    AccountNumber = AccountNumber,
                    LabelEn = model.LabelEn,
                    LabelFr = model.LabelFr,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "COA"),
                    AccountCartegoryId = model.AccountCartegoryId,
                    AccountNumberCU = model.AccountNumber.PadRight(6, '0'),
                    AccountNumberNetwork = model.AccountNumber.PadRight(6, '0') + "012".PadRight(3, '0'),
                    IsBalanceAccount = model.IsBalanceAccount,
                    ParentAccountId = model.ParentAccountId,
                    ParentAccountNumber = model.ParentAccountNumber



                };
            }
            else
            {

            }
            return chart;
        }

        private Data.ChartOfAccountManagementPosition CreateChartOfAccountManagementPosition(Data.ChartOfAccount chartOfAccount, ChartOfAccountManagementPositionFile item, string accountNumber, List<ChartOfAccountManagementPosition> modelList, string old_accountNumber,bool IsUniqueAccount)
        {


            return new Data.ChartOfAccountManagementPosition
            {
                AccountNumber = accountNumber,
                Old_AccountNumber = item.AccountNumberBapCCUL,
                New_AccountNumber = item.AccountNumber,
                IsUniqueAccount = IsUniqueAccount,
                ChartOfAccountId = chartOfAccount.Id,
                PositionNumber = "000", // (positionNumber.ToString().Length == 4) ? positionNumber.ToString().Substring(0, 3).PadLeft(3, '0') : positionNumber.ToString().PadLeft(3, '0'),
                Description = item.AccountName,
                RootDescription = chartOfAccount.LabelEn,
                TempData = accountNumber.PadRight(6, '0') + item.AccountNumber.Substring(item.AccountNumber.Length-3) + "000",
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "CMP")
            };
        }
        private List<Data.ChartOfAccountManagementPosition> CreateChartOfAccountManagementPositionForLIAISON(Data.ChartOfAccount? chartOfAccount, ChartOfAccountManagementPositionFile item, List<Branch> branches, string old_account_number)
        {
            List<Data.ChartOfAccountManagementPosition> ChartOfAccountManagementPositions = new List<Data.ChartOfAccountManagementPosition>();
            foreach (var branch in branches)
            {

                ChartOfAccountManagementPositions.Add(new Data.ChartOfAccountManagementPosition
                {
                    AccountNumber = chartOfAccount.AccountNumber,
                    Old_AccountNumber = item.AccountNumberBapCCUL,
                    New_AccountNumber = item.AccountNumber,
                    ChartOfAccountId = chartOfAccount.Id,
                    PositionNumber = branch.branchCode, // (positionNumber.ToString().Length == 4) ? positionNumber.ToString().Substring(0, 3).PadLeft(3, '0') : positionNumber.ToString().PadLeft(3, '0'),
                    Description = branch.name + " LIAISON",
                    RootDescription = chartOfAccount.LabelEn,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "CMP")
                });

            }

            return ChartOfAccountManagementPositions;
        }
        public string GetUniqueNumber(string number, List<Data.ChartOfAccountManagementPosition> modelList,string accountNumber)
        {
            int uniqueNumber = int.Parse(number);
            try
            {

                while (true)
                {
                    string formattedNumber = uniqueNumber.ToString().PadRight(3,'0') + "000";
                      bool isUnique = !modelList.Any(num => num.New_AccountNumber == accountNumber.PadRight(6, '0') + formattedNumber);

                    if (isUnique)
                    {
                        return accountNumber.PadRight(6, '0') +uniqueNumber.ToString().PadRight(3, '0') + "000";
                    }
                    else
                    {

                    }

                    uniqueNumber++;
                }
            }
            catch (Exception ex)
            {

                throw(ex);
            }
        }
        private Data.ChartOfAccountManagementPosition CreateChartOfAccountManagementPositionfix(Data.ChartOfAccount? chartOfAccount, ChartOfAccountManagementPositionFile item,string positionNumber, List<Data.ChartOfAccountManagementPosition> modelList, string old_account_number,  bool isUniqueToBranch)
        {
            
            string control =  "";

            control = GetUniqueNumber(positionNumber, modelList, old_account_number.Substring(0,6));


            return new Data.ChartOfAccountManagementPosition
            {
                AccountNumber = chartOfAccount.AccountNumber,
                Old_AccountNumber = item.AccountNumberBapCCUL,
                New_AccountNumber = control,
                ChartOfAccountId = chartOfAccount.Id,
                PositionNumber = positionNumber,// modelList.Where(x => x.AccountNumber == chartOfAccount.AccountNumber).Count().ToString().PadRight(3, '0'), // (positionNumber.ToString().Length == 4) ? positionNumber.ToString().Substring(0, 3).PadLeft(3, '0') : positionNumber.ToString().PadLeft(3, '0'),
                Description = item.AccountName,
                IsUniqueAccount= isUniqueToBranch,
                RootDescription = chartOfAccount.LabelEn,
                TempData = chartOfAccount.AccountNumber.PadRight(6, '0')  + modelList.Where(x => x.AccountNumber == chartOfAccount.AccountNumber).Count().ToString().PadRight(3, '0') + "000",
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "CMP")
            };
        }

        private AddAccountCommand CreateAccountEntry(UploadAccount request, AccountModelX item, string branchId,string BranchCode_code, Data.ChartOfAccountManagementPosition model, Data.ChartOfAccount chartOfAccount)
        {
            return new AddAccountCommand
            {
                AccountNumberManagementPosition = model.PositionNumber,
                AccountOwnerId = BranchId,
                AccountName = item.AccountName,
                OwnerBranchCode = BranchCode_code,
                AccountNumber = chartOfAccount.AccountNumber,
                AccountNumberNetwok = (chartOfAccount.AccountNumber.PadLeft(6, '0') + "012" + BranchCode_code).PadLeft(12, '0') + model.PositionNumber,
                AccountNumberCU = (chartOfAccount.AccountNumber.PadLeft(6, '0') + BranchCode_code).PadLeft(9, '0') + model.PositionNumber,
                ChartOfAccountManagementPositionId = model.Id,
                AccountTypeId = BranchId,
                AccountCategoryId = chartOfAccount.AccountCartegoryId
            };
        }


  
        private string GetEntryType(Data.Account accountEntity)
        {
            return accountEntity.DebitBalance == 0 ? AccountOperationType.DEBIT.ToString() : AccountOperationType.CREDIT.ToString();
        }


        private decimal retrieveCurrentBalance(AccountModelX item)
        {
            if (item.EndBalanceCredit > 0)
            {
                return item.EndBalanceCredit * -1;
            }
            return item.EndBalanceCredit;
        }

        private decimal retrieveBeginingBalance(AccountModelX item)
        {
            if (item.BeginningCreditBalance > 0)
            {
                return item.BeginningDebitBalance * -1;
            }
            return item.BeginningCreditBalance;
        }

        private bool CheckIfAccountExist(string accountNumber, string branchCode, string branchId)
        {
            return _accountRepository.FindBy(c => c.AccountNumber == accountNumber && c.BranchCode == branchCode && c.AccountOwnerId == branchId).Any();
        }


        private List<string> LoopForDifference(List<Data.Account> accounts, List<AccountModelX> accountModelList)
        {
            // return the list of accounts that are not in the list
            List<string> uploadAccounts = new List<string>();
            foreach (var account in accountModelList)
            {
                if (!accounts.Any(c => c.AccountName == account.AccountName))
                {
                    uploadAccounts.Add(account.AccountNumber);
                }
            }
            return uploadAccounts;
        }

        private bool CheckIfAccountIsCreated(string name, List<Data.Account> modelList)
        {
            //Check if chart of account name is in modelList
            return modelList.Any(c => c.AccountName == name);

        }
        private bool CheckIfChartOfAccountIsCreated(string name, List<Data.ChartOfAccountManagementPosition> modelList)
        {
            //Check if chart of account name is in modelList
            return modelList.Any(c => c.Description == name);

        }

        private PositionChart CheckIfChartOfAccountIsCreated(AccountModelX model, Data.ChartOfAccount chartOfAccount)
        {
            PositionChart positionChart = new PositionChart();
            //Check if chart of account name is in modelList
            if (model.AccountNumber.Contains("45100000000"))
            {
                var list = _chartOfAccountManagementPositionRepository.FindBy(c => c.AccountNumber == "451").ToList();
                if (list.Any())
                {
                    var modelx = list.FirstOrDefault();
                    positionChart.Isperesent = true;
                    positionChart.Is451peresent = true;
                    positionChart.chartOfAccountID = modelx.ChartOfAccountId;
                    positionChart.ChartOfAccountManagementPositionId = modelx.Id;
                    positionChart.ChartOfAccountManagementPosition = modelx;
                }

            }
            else
            {
                if (model.ChartofAccount.Contains("18112"))
                {
                    var listx = _chartOfAccountManagementPositionRepository.FindBy(c => c.AccountNumber == "18112").ToList();
                    if (listx.Any())
                    {
                        var modelx = listx.FirstOrDefault();
                        positionChart.Isperesent = true;
                        positionChart.Is451peresent = false;
                        positionChart.Is18112peresent = true;
                        positionChart.chartOfAccountID = modelx.ChartOfAccountId;
                        positionChart.ChartOfAccountManagementPositionId = modelx.Id;
                        positionChart.ChartOfAccountManagementPosition = modelx;
                    }

                }
                else
                {
                    var list = _chartOfAccountManagementPositionRepository.FindBy(c => c.ChartOfAccountId == chartOfAccount.Id).ToList();
                    if (list.Any())
                    {
                        var modelx = list.FirstOrDefault();
                        positionChart.Isperesent = true;
                        positionChart.chartOfAccountID = modelx.ChartOfAccountId;
                        positionChart.ChartOfAccountManagementPositionId = modelx.Id;
                        positionChart.ChartOfAccountManagementPosition = modelx;
                        positionChart.Is451peresent = false;
                    }
                }

            }

            return positionChart;

        }
    }
}


