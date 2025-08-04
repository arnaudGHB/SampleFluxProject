using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddProductAccountBookCommand.
    /// </summary>
public class AddProductAccountingBookCommand : IRequest<ServiceResponse<ProductAccount>>
    {

        public string ProductAccountBookName { get; set; }//Saving Light, Mini Saving Account etc ....
        public List<ProductAccountBookDetail> ProductAccountBookDetails { get; set; }
        public string ProductAccountBookId { get; set; }
        public string ProductAccountBookType { get; set; }//Loan_Product,Teller,Saving_Product
        
    }


    public class ProductAccount 
    {

        public string AccountNumber { get; set; }// name of the attribute
      public string ChartOfAccountManagementPositionId { get;   set; }
        public List<AccountNotUpdated>  notUpdateds { get; set; }
    }


    public class ProductAccountBookDetail
    {

        public string Name { get; set; }// name of the attribute
        public string ChartOfAccountId { get; set; }

    }


    public class AddProductAccountBookForSystemCommand : IRequest<ServiceResponse<ProductAccountingBookDto>>
    {

        public string Name { get; set; }//Saving Light, Mini Saving Account etc ....
        public  string ChartOfAccountIds { get; set; }
        public string ProductAccountBookId { get; set; }
        public string ProductAccountBook { get; set; }

    }


}