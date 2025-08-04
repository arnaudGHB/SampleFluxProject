using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class ChartOfAccount : BaseEntity
    {
        public string Id { get; set; }
        // Unique account number 
        public string AccountNumber { get; set; } //37123

 

        // Descriptive label for the account
        public string LabelFr { get; set; }="";
        public string LabelEn { get; set; } = "";
        // Flags if this is a balance sheet account
        public bool IsBalanceAccount { get; set; }
        // Specifies if account balance can go negative
        public string? AccountCartegoryId { get; set; }
        public bool? IsDebit { get; set; }
       
        public bool? HasManagementPostion { get; set; }
        public string? ParentAccountNumber { get; set; }//3712
        public string? ParentAccountId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime MigrationDate { get; set; }
        public string AccountNumberNetwork { get; set; } = "";
        public string AccountNumberCU { get; set; } = "";
        public string? Account1 { get; set; }
        public string? Account2 { get; set; }

        public string? Account3 { get; set; }
        public string? Account4 { get; set; }
        public string? Account5 { get; set; }
        public string? Account6 { get; set; }
        public string? Account7 { get; set; }
        public virtual AccountCategory AccountCartegory { get; set; }
        public virtual ICollection<ChartOfAccountManagementPosition> ChartOfAccountManagementPositions { get; set; }
        //public virtual ICollection<CorrespondingMapping> CorrespondingMappings { get; set; }
        public virtual ICollection<DocumentReferenceCode> DocumentReferenceCodes { get; set; }
        public static string Get12PositionRepresentation(int number)
        {
            // Convert the number to a string
            string numberString = number.ToString();

            // Pad the string with leading zeros if it has less than 12 characters
            if (numberString.Length < 12)
            {
                numberString = numberString.PadLeft(12, '0');
            }

            return numberString;
        }

        public static ChartOfAccount SetChartOfAccountEntity(ChartOfAccount entity, UserInfoToken _userInfoToken)
        {


            entity.CreatedDate = DateTime.Now.ToLocalTime();
            entity.ModifiedDate = DateTime.Now.ToLocalTime();
            entity.Id = Guid.NewGuid().ToString();


            return entity;
        }
        public static string AccountNumberEntity(string AccountNumber, string? entity, int number)
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
        public static ChartOfAccount UpdateAccountEntity( ChartOfAccount entity)
        {
          // entity.ParentAccountNumber =entity.AccountNumber.Length==1 ? entity.AccountNumber=="0"?"ROOT": "0": entity.AccountNumber.Substring(entity.AccountNumber.Length-1);
          entity.TempData = entity.AccountNumber.PadRight(6, '0');
            entity.AccountNumberCU= entity.AccountNumber.PadRight(6,'0'  )+"012000";
            entity.AccountNumberNetwork = entity.AccountNumber.PadRight(6, '0') + "000000";
            entity.Account1 = AccountNumberEntity(entity.AccountNumber, entity.Account1, 1);
            entity.Account7 = AccountNumberEntity(entity.AccountNumber, entity.Account7, 7);
         
            entity.Account6 = AccountNumberEntity(entity.AccountNumber, entity.Account6, 6);
            entity.Account5 = AccountNumberEntity(entity.AccountNumber, entity.Account5, 5);
            entity.Account4 = AccountNumberEntity(entity.AccountNumber, entity.Account4, 4);
            entity.Account3 = AccountNumberEntity(entity.AccountNumber, entity.Account3, 3);
            entity.Account2 = AccountNumberEntity(entity.AccountNumber, entity.Account2, 2);
            if (entity.AccountNumber.Length == 1)
            {
                if (entity.AccountNumber == "0")
                {
                    entity.ParentAccountNumber = "ROOT";
                                }
                else
                {
                    entity.ParentAccountNumber = "0";
                }
            }
            else
            {
                entity.ParentAccountNumber = entity.AccountNumber.Substring(0, entity.AccountNumber.Length - 1);
            }

            return entity;
        }
        public static List<ChartOfAccount> SetAccountCategoriesEntities(string JsonString, UserInfoToken _userInfoToken)
        {
            var ChartOfAccounts = JsonConvert.DeserializeObject<List<ChartOfAccount>>(JsonString);
            List<ChartOfAccount> listOfChartOfAccounts = new List<ChartOfAccount>();
            foreach (ChartOfAccount item in ChartOfAccounts)
            {
                ChartOfAccount entity = UpdateAccountEntity(item);
                listOfChartOfAccounts.Add(entity);
            }
            return listOfChartOfAccounts;
        }

    }
}
