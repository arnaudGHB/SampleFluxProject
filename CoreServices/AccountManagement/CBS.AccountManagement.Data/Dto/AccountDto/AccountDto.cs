using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.Data.Enum;

namespace CBS.AccountManagement.Data
{
    public class AccountingOperations
    {
        public string CashInHand { get; set; }
        public string CashInVault { get; set; }
        public string MTNMoMoBranch { get; set; }
        public string OMBranch { get; set; }
        public string MTNMoMoHO { get; set; }
        public string OMBranchHo { get; set; }
    }
    public class AccountDto
    {
        public string Id { get; set; }
        // Account Number
        public string? AccountNumber { get; set; }
        // Account Holder
        public string? AccountName { get; set; }
        // Beginning Balance
        public string BranchCode { get; set; }
        public string AccountNumberManagementPosition { get; set; }
        public decimal BeginningBalance { get; set; } = 0;

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

        public string LiaisonId { get; set; } = "1";

        public string ChartOfAccountManagementPositionId { get; set; } = "1";
        public string AccountCategoryId { get; set; } = "1";
        public string? AccountOwnerId { get; set; } = "1";
        public string? AccountOwnerID { get; internal set; }
        public string AccountNumberNetwork { get; set; }
        public string AccountNumberCU { get; set; }
        public string AccountNumberReference { get; set; }
        public DateTime MigrationDate { get; set; }
        public string? Account1 { get; set; }
        public string? Account2 { get; set; }

        public string? Account3 { get; set; }
        public string? Account4 { get; set; }
        public string? Account5 { get; set; }
        public string? Account6 { get; set; }
        public string BranchId { get;   set; }
        public string BankId { get;   set; }
        public string ParentAccountNumber { get;   set; }
        public string LabelEn { get;   set; }
        public string TempData { get; set; }
        public static List<JsData> ConvertToTreeNodeList(List<AccountDto> chartOfAccounts, string language = "0")
        {
            var treeNodes = new List<JsData>();
            List<JsData> AccountTreeNodes = new List<JsData>();
            foreach (var account in chartOfAccounts)
            {
                var treeNode = new JsData
                {
                    id = account.AccountNumber ?? "", // Assuming ParentAccountId is used for the id
                    text = /*$"{account.AccountNumber} - {account.LabelFr}",//:*/ $"{account.AccountNumber} - {account.LabelEn}:{(account.CreditBalance-account.DebitBalance).ToString("N")} FCFA",
                    children = new List<JsData>(),
                    state = new State { opened = false, disabled = false, selected = false },
                    icon = "mdi mdi-folder-outline",
                    parentId = account.ParentAccountNumber ?? "",

                    li_attr = new Dictionary<string, object>
                        {
                            {"class", "custom-node" },
                            {"data-bs-toggle","modal"},
                          {"data-bs-target","#largeModal"}

                        },
                    a_attr = new Dictionary<string, object>
                        {
                             {"class", "parent"},
                             {"Id", "parent_"+account.AccountNumber}
                        }
                };

                treeNodes.Add(treeNode);
            }

            var reModel = JsData.BuildTree(treeNodes);

            return reModel;
        }


    }

    public class InfoAccount
    {
        public string Id { get; set; }
        // Account Number
        public string? AccountNumber { get; set; }
        // Account Holder
        public string? AccountName { get; set; }
        public string Type { get; set; }
        public decimal CurrentBalance { get; set; }
    }
        public class DashbaordDto
    {
        public AccountInfo? CashInHand { get; set; }
        // Account Holder
        public AccountInfo? CashInBank { get; set; }
        // Beginning Balance
        public AccountInfo? TotalShares { get; set; }
        public AccountInfo? TotalSavings { get; set; }
        public AccountInfo? TotalDeposit { get; set; }
        public AccountInfo? TotalExpense { get; set; }
        public AccountInfo? TotalIncome { get; set; }
        public AccountInfo? TotalLiquity { get; set; }
    }
    public class AccountInfo
    {
        public double Balance { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }

        public class AccountUploadDto
    
    {
        public string Balance { get; set; }
       
        public string? Message { get; set; }
 
        public string? AccountNumber { get; set; }
 
        public string? AccountName{ get; set; }

        public AccountUploadDto(  string? AccountNumber, string? AccountName, string? Balance , bool isExisting = false)
        {
            if (isExisting)
            {
                Message = "\n " + AccountNumber.ToString() + "-" + AccountName.ToString() + "new account has been created";
            }
            else
            {
                Message = "\n " + AccountNumber.ToString() + "-" + AccountName.ToString() + " Already exist in the system. current balance is " + Balance.ToString();
            }

   
            this.AccountNumber = AccountNumber;
            this.AccountName = AccountName;
            
          
        }
       
    }

 
}