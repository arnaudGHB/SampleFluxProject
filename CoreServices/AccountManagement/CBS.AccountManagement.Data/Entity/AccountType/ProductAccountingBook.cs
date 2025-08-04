using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CBS.AccountManagement.Data
{
    public class ProductAccountingBook : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string? ProductAccountingBookId { get; set; }
        public string? ChartOfAccountId { get; set; } //ChartOfAccountId 
        public string? ChartOfAccountManagementPositionId { get; set; }
        public string? Name { get; set; }
        public string? ProductAccountingBookName { get; set; }
        public string? AccountTypeId { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string ProductType { get; set; }
        public virtual AccountType? AccountType { get; set; }

        public virtual ChartOfAccountManagementPosition? ChartOfAccountManagementPosition { get; set; }
        public virtual OperationEventAttributes? OperationEventAttributes { get; set; }



        public static Attributes productAccountingBooking(List<AccountTypeDetail> chartOfAccountIds, string OperationEventAttributeId, string OperationAccountTypeId, string productName, string operationAccountType, string AccountType, UserInfoToken userInfoToken,string OperationEventId)
        {
            Attributes listProduct = new Attributes();
            foreach (var item in chartOfAccountIds)
            {
                listProduct.ProductAccountingBooksList.Add(new ProductAccountingBook
                {
                    Id = OperationAccountTypeId+'@' + item.Name,
                    Name= productName,
                    ProductAccountingBookName = productName,
                    ProductAccountingBookId = OperationAccountTypeId,
                    ChartOfAccountId = item.ChartOfAccountId,
                    ChartOfAccountManagementPositionId = item.ChartOfAccountId,
                    OperationEventAttributeId = OperationEventAttributeId,
                    AccountTypeId = AccountType,
                    ProductType = operationAccountType
                });
                listProduct.OperationEventAttributesList.Add(new OperationEventAttributes
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = productName + "@" + item.Name,
                    Description = productName,
                    OperationEventAttributeCode = productName + "@" + item.Name,
                    OperationEventId = OperationEventId,
                    CreatedBy = userInfoToken.Id,
                    CreatedDate = DateTime.Now,

                });
            }
            return listProduct;
        }

        public static ProductAccountingBook CreateproductAccountingBooking(string chartOfAccountIds, string OperationEventAttributeId, string OperationAccountTypeId, string productName, string AccountType, UserInfoToken userInfoToken)
        {
            ProductAccountingBook book = new ProductAccountingBook();
            book.Id = Guid.NewGuid().ToString();
            book.ProductAccountingBookName = productName;
            book.ProductAccountingBookId = OperationAccountTypeId;
            book.ChartOfAccountId = chartOfAccountIds;
            book.OperationEventAttributeId = OperationEventAttributeId;
            book.AccountTypeId = AccountType;
            book.CreatedBy = userInfoToken.Id;
            book.CreatedDate = DateTime.Now;
            book.ModifiedDate = DateTime.Now;
            book.ModifiedBy = userInfoToken.Id;
            book.IsDeleted = false;
            book.DeletedBy = null;
            book.DeletedDate = DateTime.Now;
            return book;
        }

    

    }
    public class Attributes
    {

        public List<ProductAccountingBook> ProductAccountingBooksList { get; set; } = new();// name of the attribute
        public List<OperationEventAttributes> OperationEventAttributesList { get; set; } = new();


    }
    public class AccountTypeDetail
    {

        public string Name { get; set; }// name of the attribute
        public string ChartOfAccountId { get; set; }
        

    }
}
