using CBS.NLoan.Data.Data;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Command
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
        public string AccountNumber { get; set; }
        public string ChartOfAccountManagementPositionId { get; set; }
    }
}
