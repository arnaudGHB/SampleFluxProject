using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class AddAccountingAccountTypePICallCommand : IRequest<ServiceResponse<AddAccountingAccountTypeResponse>>
    {
        /// <summary>
        /// Product Name like Saving Light, Mini Saving Account
        /// </summary>
        public string ProductAccountBookName { get; set; }//Saving Light, Mini Saving Account etc ....
        public List<ProductAccountBookDetail> ProductAccountBookDetails { get; set; }=new List<ProductAccountBookDetail>();
        /// <summary>
        /// Product Id like 1,2,3
        /// </summary>
        public string ProductAccountBookId { get; set; }
        /// <summary>
        /// Product types like Loan_Product,Teller,Saving_Product
        /// </summary>
        public string ProductAccountBookType { get; set; }//Loan_Product,Teller,Saving_Product
    }

    public class AddAccountingAccountTypeResponse
    {
        public string AccountNumber { get; set; }// name of the attribute
        public string ChartOfAccountManagementPositionId { get; set; }
        public List<ProductAccountNotUpdated> NotUpdateds { get; set; }
        public AddAccountingAccountTypeResponse()
        {
            NotUpdateds=new List<ProductAccountNotUpdated>();
        }
    }
    public class ProductAccountNotUpdated
    {

        public string AccountId { get; set; }// name of the attribute
        public string AccountName { get; set; }
    }
}
