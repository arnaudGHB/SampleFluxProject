using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.Data
{
     

    public class ProductAccountingBookDto
    {
        public string Id { get; set; }
        public string? ProductAccountingBookId { get; set; }
        public string? ChartOfAccountId { get; set; } //ChartOfAccountId 
        public string? ChartOfAccountManagementPositionId { get; set; }
        public string? Name { get; set; }
        public string? ProductAccountingBookName { get; set; }
        public string? AccountTypeId { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string ProductType { get; set; }
    }

    public class AccountTypeCreationDto
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string OperationAccountTypeId { get; set; }
        public string OperationAccountType { get; set; }
    }

    public class AccountTypeDto
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string OperationAccountTypeId { get; set; }
        public string OperationAccountType { get; set; }
    }

    public class ProductAccountConfiguration
    {
        public string Id { get; set; }
        public string? OperationEvent { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Key { get; set; }
    }
}