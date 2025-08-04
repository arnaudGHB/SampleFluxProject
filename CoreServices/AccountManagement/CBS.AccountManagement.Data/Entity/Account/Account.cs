using CBS.AccountManagement.Data.Enum;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.AccountManagement.Data
{
    public enum BalanceNature
    {
        Debit,
        Credit,
        Mixed,
        Unknown
    }

    public class AccountBalanceAnalyzer
    {
        public static BalanceNature DetermineBalanceNature(int accountClass)
        {
            switch (accountClass)
            {
                case 40: // Fournisseurs
                    return BalanceNature.Debit;

                case 41: // Instruments de paiement
                    return BalanceNature.Mixed;

                case 42: // Personnel
                    return BalanceNature.Credit;

                case 43: // État
                    return BalanceNature.Debit;

                case 44: // Sociétaires et actionnaires
                    return BalanceNature.Mixed;

                case 45: // Comptes de liaison
                    return BalanceNature.Credit;

                case 46: // Autres débiteurs et créditeurs
                    return BalanceNature.Debit;

                case 47: // Comptes de régularisation
                    return BalanceNature.Debit;

                case 48: // Créances diverses en souffrance
                    return BalanceNature.Debit;

                case 49: // Provisions
                    return BalanceNature.Credit;

                default:
                    return BalanceNature.Unknown;
            }
        }
    }
    public class Account : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        // Account Number
        public string? AccountNumber { get; set; }
        // Account Holder
        public string? AccountName { get; set; }
        // Beginning Balance
        public string BranchCode { get; set; }
        public string AccountNumberManagementPosition { get; set; }
        public decimal BeginningBalance { get; set; } = 0;
        public decimal BeginningBalanceDebit { get; set; } = 0;
        public decimal BeginningBalanceCredit { get; set; } = 0;
        public decimal DebitBalance { get; set; } = 0;
        // Current Balance
        public decimal CurrentBalance { get; set; } = 0;
        public decimal CreditBalance { get; set; } = 0;
        // LastBalance Balance
        public decimal LastBalance { get; set; } = 0;
        public string? BookingDirection { get; set; }
        // Status
        public string Status { get; set; }
        // BranchId  

        public bool CanBeNegative { get; set; } = false;

        public bool IsUniqueToBranch { get; set; } = false;

        public string LiaisonId { get; set; } = "1";

        public string ChartOfAccountManagementPositionId { get; set; } = "1";
        public string AccountCategoryId { get; set; } = "1";
        public string? AccountOwnerId { get; set; } = "1";
        public string AccountNumberNetwork { get; set; }
        public string AccountNumberReference { get; set; }
        public string AccountNumberCU { get; set; }
        public string AccountMappingStatus { get; set; }
        public DateTime MigrationDate { get; set; }
        public string? Account1 { get; set; }
        public string? Account2 { get; set; }

        public string? Account3 { get; set; }
        public string? Account4 { get; set; }
        public string? Account5 { get; set; }
        public string? Account6 { get; set; }
        public virtual ChartOfAccountManagementPosition ChartOfAccountManagementPosition { get; set; } = null;


        public static List<Account> SetAccountEntities(List<Account> accounts, UserInfoToken _userInfoToken)
        {
            List<Account> accountLists = new List<Account>();
            foreach (Account entity in accounts)
            {
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.CreatedDate = DateTime.Now.ToLocalTime();
                entity.ModifiedDate = DateTime.Now.ToLocalTime();
                entity.Id = Guid.NewGuid().ToString();
                accountLists.Add(entity);
            }
            return accountLists;
        }
        public Account SetAccountEntity(Account entity, string AccountCategoryId, string AccountNumber, string AccountHolder, string ChartOfAccountManagementPositionId, string accountOwnerId, string AccountNumberCU, string BranchCode, string AccountNumberManagementPosition)
        {

            entity.BranchCode = BranchCode;
            entity.AccountNumberManagementPosition = AccountNumberManagementPosition.PadRight(3, '0');
            entity.AccountNumber = AccountNumber;
            entity.AccountName = AccountHolder;
            entity.ChartOfAccountManagementPositionId = ChartOfAccountManagementPositionId;
            entity.AccountMappingStatus = AccountMappingStatusEnum.MappedToSystem.ToString();
            entity.AccountOwnerId = accountOwnerId;
            entity.AccountCategoryId = AccountCategoryId;
            entity.AccountNumberNetwork = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0') + BranchCode.PadRight(12, '0') + "12".PadRight(3, '0');
            entity.AccountNumberCU = AccountNumberCU;// AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0') + BranchCode.PadRight(3, '0');
            entity.AccountNumberReference = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0');
            entity.Account2 = AccountNumberEntity(AccountNumber, entity.Account2, 2);
            entity.Account3 = AccountNumberEntity(AccountNumber, entity.Account3, 3);
            entity.Account4 = AccountNumberEntity(AccountNumber, entity.Account4, 4);
            entity.Account5 = AccountNumberEntity(AccountNumber, entity.Account5, 5);
            entity.Account6 = AccountNumberEntity(AccountNumber, entity.Account6, 6);
            entity.Account1 = AccountNumberEntity(AccountNumber, entity.Account1, 1);
            entity.TempData = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition;
            entity.BeginningBalance = 0;
            entity.CurrentBalance = 0;
            entity.LastBalance = 0;
            entity.Status = AccountStatus.Active.ToString();


            return entity;
        }

        public Account SetAccountEntity(Account entity, string AccountCategoryId, string AccountNumber, string AccountHolder, string ChartOfAccountManagementPositionId, string accountOwnerId, string BankCOde, string BranchCode, string AccountNumberManagementPosition, string LiaisonId)
        {

            entity.BranchCode = BranchCode;
            entity.AccountNumberManagementPosition = AccountNumberManagementPosition.PadRight(3, '0');
            entity.AccountNumber = AccountNumber;
            entity.AccountName = AccountHolder;
            entity.AccountMappingStatus = AccountMappingStatusEnum.MappedToSystem.ToString();
            entity.ChartOfAccountManagementPositionId = ChartOfAccountManagementPositionId;
            entity.AccountOwnerId = accountOwnerId;
            entity.AccountNumberNetwork = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0') + BranchCode.PadRight(12, '0') + BankCOde.PadRight(3, '0');
            entity.AccountNumberCU = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0') + BranchCode.PadRight(3, '0');
            entity.AccountNumberReference = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0');
            entity.Account2 = AccountNumberEntity(AccountNumber, entity.Account2, 2);
            entity.Account3 = AccountNumberEntity(AccountNumber, entity.Account3, 3);
            entity.Account4 = AccountNumberEntity(AccountNumber, entity.Account4, 4);
            entity.Account5 = AccountNumberEntity(AccountNumber, entity.Account5, 5);
            entity.Account6 = AccountNumberEntity(AccountNumber, entity.Account6, 6);
            entity.Account1 = AccountNumberEntity(AccountNumber, entity.Account1, 1);
            entity.TempData = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition;
            entity.LiaisonId = LiaisonId;
            entity.BeginningBalance = 0;
            entity.CurrentBalance = 0;
            entity.LastBalance = 0;
            entity.Status = AccountStatus.Active.ToString();


            return entity;
        }


        public Account SetAccount451Entity(string SourceBranchCode, Account entity, string AccountCategoryId, string AccountNumber, string AccountNumberCU, string AccountHolder, string ChartOfAccountManagementPositionId, string accountOwnerId, string BankCOde, string DestinationBranchCode, string AccountNumberManagementPosition, string LiaisonId)
        {

            entity.BranchCode = SourceBranchCode;
            entity.AccountNumberManagementPosition = AccountNumberManagementPosition.PadRight(3, '0');
            entity.AccountNumber = AccountNumber;
            entity.AccountName = AccountHolder;
            entity.AccountMappingStatus = AccountMappingStatusEnum.MappedToSystem.ToString();
            entity.ChartOfAccountManagementPositionId = ChartOfAccountManagementPositionId;
            entity.AccountOwnerId = accountOwnerId;
            entity.BranchId = accountOwnerId;
            entity.AccountNumberNetwork = AccountNumber.PadRight(6, '0') + SourceBranchCode + BranchCode.PadRight(6, '0') ;
            entity.AccountNumberCU = AccountNumberCU;
            entity.AccountNumberReference = AccountNumber.PadRight(6, '0');
            entity.Account1 = AccountNumberEntity(AccountNumber, entity.Account1, 1);
            entity.Account2 = AccountNumberEntity(AccountNumber, entity.Account2, 2);
            entity.Account3 = AccountNumberEntity(AccountNumber, entity.Account3, 3);
            entity.Account4 = AccountNumberEntity(AccountNumber, entity.Account4, 4);
            entity.Account5 = AccountNumberEntity(AccountNumber, entity.Account5, 5);
            entity.Account6 = AccountNumberEntity(AccountNumber, entity.Account6, 6);
            entity.Account1 = AccountNumberEntity(AccountNumber, entity.Account1, 1);
            entity.TempData = AccountNumber.PadRight(6, '0') + SourceBranchCode.PadRight(3, '0') + BranchCode.PadRight(3, '0');
            entity.LiaisonId = LiaisonId;
            entity.BeginningBalance = 0;
            entity.CurrentBalance = 0;
            entity.LastBalance = 0;
            entity.Status = AccountStatus.Active.ToString();


            return entity;
        }


        public Account SetAccountEntityBranchCode(AccountMappingStatusEnum accountStatus, Account entity, string accountNumber, string accountName, string chartOfAccountId, string accountOwnerId, string bankCOde, string branchCode, string accountNumberManagementPosition, decimal beginningBalance, decimal beginningBalanceDebit, decimal beginningBalanceCredit, decimal movementDebit, decimal movementCredit, decimal currentBalance, string bookingDirection, string liaisonId)

        {

       
            entity.BeginningBalance = 0;
            entity.BranchCode = branchCode;
            entity.AccountNumberManagementPosition = accountNumberManagementPosition.PadRight(3, '0');
            entity.AccountNumber = accountNumber;
            entity.AccountName = accountName;
            entity.ChartOfAccountManagementPositionId = chartOfAccountId;
            entity.AccountOwnerId = accountOwnerId;
            entity.LiaisonId = LiaisonId;
            entity.AccountNumberNetwork = accountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0') + branchCode.PadRight(3, '0') + bankCOde.PadRight(3, '0');
            entity.AccountNumberCU = entity.AccountNumberCU;
            entity.AccountNumberReference = AccountNumber.PadRight(6, '0') + branchCode.PadRight(3, '0');
            entity.AccountMappingStatus = accountStatus.ToString();
            entity.Account1 = AccountNumberEntity(AccountNumber, entity.Account1, 1);
            entity.Account2 = AccountNumberEntity(AccountNumber, entity.Account2, 2);
            entity.Account3 = AccountNumberEntity(AccountNumber, entity.Account3, 3);
            entity.Account4 = AccountNumberEntity(AccountNumber, entity.Account4, 4);
            entity.Account5 = AccountNumberEntity(AccountNumber, entity.Account5, 5);
            entity.Account6 = AccountNumberEntity(AccountNumber, entity.Account6, 6);
            entity.TempData = accountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition;
            entity.BeginningBalanceCredit = beginningBalanceCredit;
            entity.BeginningBalanceDebit = beginningBalanceDebit;
            entity.BookingDirection = bookingDirection;
            entity.LiaisonId = liaisonId;
            entity.CreditBalance = movementCredit > movementDebit ? CurrentBalance : 0;
            entity.DebitBalance = movementDebit > movementCredit ? CurrentBalance : 0 ;
                entity.CurrentBalance = currentBalance;
            entity.BranchId= accountOwnerId;
            entity.BeginningBalance = beginningBalanceCredit > beginningBalanceDebit ? beginningBalanceCredit : beginningBalanceDebit;


            entity.LastBalance = 0;

            entity.LastBalance = 0;
            entity.Status = AccountStatus.Active.ToString();


            return entity;
        }

        public Account SetAccountEntityMG(AccountMappingStatusEnum accountStatus, Account entity, string AccountNumber, string AccountHolder, string ChartOfAccountId, string accountOwnerId, string BankCOde, string BranchCode, string AccountNumberManagementPosition, decimal BeginningBalance, decimal BeginningBalanceDebit, decimal BeginningBalanceCredit, decimal movementDebit, decimal movementCredit, decimal CurrentBalance, string BookingDirection, string LiaisonId)

        {
            if (AccountNumber.StartsWith("571"))
            {
                AccountNumberManagementPosition = "0";
            }
            entity.BeginningBalance = 0;
            entity.BranchCode = BranchCode;
            entity.AccountNumberManagementPosition = AccountNumberManagementPosition.PadRight(3, '0');
            entity.AccountNumber = AccountNumber;
            entity.AccountName = AccountHolder;
            entity.ChartOfAccountManagementPositionId = ChartOfAccountId;
            entity.AccountOwnerId = accountOwnerId;
            entity.LiaisonId = LiaisonId;
            entity.AccountMappingStatus = accountStatus.ToString();
            entity.AccountNumberNetwork = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0') + BranchCode.PadRight(12, '0') + BankCOde.PadRight(3, '0');
            entity.AccountNumberCU = entity.AccountNumberCU;
            entity.AccountNumberReference = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition.PadRight(3, '0');
            entity.Account1 = AccountNumberEntity(AccountNumber, entity.Account1, 1);
            entity.Account2 = AccountNumberEntity(AccountNumber, entity.Account2, 2);
            entity.Account3 = AccountNumberEntity(AccountNumber, entity.Account3, 3);
            entity.Account4 = AccountNumberEntity(AccountNumber, entity.Account4, 4);
            entity.Account5 = AccountNumberEntity(AccountNumber, entity.Account5, 5);
            entity.Account6 = AccountNumberEntity(AccountNumber, entity.Account6, 6);
            entity.TempData = AccountNumber.PadRight(6, '0') + entity.AccountNumberManagementPosition;
            entity.BeginningBalanceCredit = BeginningBalanceCredit;
            entity.BeginningBalanceDebit = BeginningBalanceDebit;
            entity.BookingDirection = BookingDirection;
            entity.CreditBalance = BeginningBalanceDebit+movementDebit > BeginningBalanceCredit+movementCredit ? 0 : CurrentBalance;
            entity.DebitBalance = BeginningBalanceDebit + movementDebit > BeginningBalanceCredit + movementCredit ? CurrentBalance : 0;
            entity.CurrentBalance = CurrentBalance;
            entity.BeginningBalance =BeginningBalanceCredit > BeginningBalanceDebit ? BeginningBalanceCredit : BeginningBalanceDebit;

            entity.LastBalance = 0;
            entity.Status = AccountStatus.Active.ToString();


            return entity;
        }

        private decimal DetermineBalance(Account entity, decimal beginningBalanceCredit, decimal beginningBalanceDebit)
        {
            decimal balance = 0;
            if (IsCreditNormal(entity.AccountNumber))
            {
                balance = beginningBalanceCredit;
            }
            else if (IsdebitNormal(entity.AccountNumber))
            {
                balance = beginningBalanceDebit;
            }
            else
            {
                var number = entity.AccountNumber.Substring(0, 2);
                if (AccountBalanceAnalyzer.DetermineBalanceNature(Convert.ToInt32(number)) == BalanceNature.Credit)
                {

                    balance = entity.CurrentBalance - entity.CreditBalance + entity.DebitBalance;
                }
                else

                {

                    balance = entity.CurrentBalance - entity.DebitBalance + entity.CreditBalance;
                }


                if (balance<0)
                {
                    balance = 0;
                }


            }
            return balance;
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

        public static string CalculateIBAN(string countryCode, string bankCode, string accountNumber)
        {
            string iban = countryCode + bankCode + accountNumber;
            iban = iban.Replace(" ", string.Empty);

            string converted = string.Empty;

            foreach (char c in iban)
            {
                int asciiValue = Convert.ToInt32(c);

                if (asciiValue >= 48 && asciiValue <= 57)
                    converted += c.ToString();
                else
                    converted += (asciiValue - 55).ToString();
            }

            int modulus = Convert.ToInt32(converted.Substring(0, 1));
            for (int i = 1; i < converted.Length; i++)
            {
                int next = Convert.ToInt32(converted.Substring(i, 1));
                modulus = (modulus * 10 + next) % 97;
            }

            return countryCode + (98 - modulus) + iban;
        }

        public string AccountNumberEntity(string AccountNumber, string? entity, int number)
        {

            if (!string.IsNullOrEmpty(AccountNumber) && AccountNumber.Length >= number)
            {
                entity = AccountNumber.Substring(0, number);
            }
            else
            {
                // Handle cases where AccountNumber is null, empty, or has fewer than 6 characters
                entity = AccountNumber ?? string.Empty;
            }
            return entity;
        }

    }
    public class AccountModel
    {
        public string AccountNumberNetwork { get; set; } = "";

        public string AccountNumberCU { get; set; } = "";
        public string ChartofAccount { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BranchCode { get; set; }

        public decimal BeginningBalance { get; set; }

        public decimal CurrentBalance { get; set; }

        public string CreatedDate { get; set; }

        public AccountModel()
        {

        }
        public AccountModel(string AccNumber, string AccName, string chartofaccount, string DateCreated, decimal beginningBalance, decimal currentBalance, string branchCode)
        {

            AccountNumber = AccNumber;
            AccountName = AccName;
            BeginningBalance = beginningBalance;
            CurrentBalance = currentBalance;
            CreatedDate = DateCreated;
            ChartofAccount = chartofaccount;
            BranchCode = branchCode;
        }

    }
    public class AccountProcessingState
    {
        public decimal Balance { get; set; }
        public decimal BalanceBefore { get; set; }
    }

    public class BSAccount
    {

        public string? Reference { get; set; }
        // Account Holder
        public string? Description { get; set; }

        public double Amount { get; set; }

        public string Cartegory { get; set; }



    }

    public class IncomeExpenseAccount
    {


    }
}