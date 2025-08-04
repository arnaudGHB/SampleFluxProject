namespace CBS.AccountManagement.MediatR.Commands
{
    public class ChartOfAccountManagementPositionFile
    {
 

        public ChartOfAccountManagementPositionFile(string? accountNumberOld, string? accountNumber, string? accountName, string IsUnique)
        {
            IsUniqueToBranch = IsUnique.ToUpper() == "YES";
            AccountName = accountName;
            AccountNumber = accountNumber ;
            AccountNumberBapCCUL = accountNumberOld;
        }
        public bool IsUniqueToBranch { get; set; }
//        public string BranchId { get; set; }
        public string AccountNumberBapCCUL { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName{ get; set; }
    }

    public class ChartOfAccountManagementPositionFile2
    {


        public ChartOfAccountManagementPositionFile2(string? accountNumber, string? accountName)
        {
          
            AccountName = accountName;
            AccountNumber = accountNumber;
   
        }
    
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }

    public class ChartOfAccountFile
    {


        public ChartOfAccountFile( string? accountNumber, string? accountName)
        {
            AccountName = accountName;
            AccountNumber = accountNumber;

        }

       
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }
}