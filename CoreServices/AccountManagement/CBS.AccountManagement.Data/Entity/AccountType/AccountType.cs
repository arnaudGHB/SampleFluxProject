using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace CBS.AccountManagement.Data
{
    public class AccountType : BaseEntity
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string OperationAccountTypeId { get; set; }
        public string OperationAccountType { get; set; }//Loan_Product,Teller,Saving_Product

        public ICollection<ProductAccountingBook>? ProductAccountingBooks { get; set; }

    }
}