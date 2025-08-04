using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class ChartOfAccountStateDto
    {
        public string Id { get; set; }
        public string GeneralRepresentation { get; set; }
    }

    public class ChartOfAccountManagementPositionDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string RootDescription { get; set; }
        public string ChartOfAccountId { get; set; }
        public string PositionNumber { get; set; }
        public string Level_Management { get; set; }
        public string AccountNumber { get; set; }
        public string? Old_AccountNumber { get; set; }
        public string? New_AccountNumber { get; set; }
        public string TempData { get; set; }
    }


    public class  AccountProduct
    {
        public string Id { get; set; }
        public string OperationEvent { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

    }

    public class ProductAccountingChart
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public List<AccountProduct> AccountingChart { get; set; } = new List<AccountProduct>();
    }
}
