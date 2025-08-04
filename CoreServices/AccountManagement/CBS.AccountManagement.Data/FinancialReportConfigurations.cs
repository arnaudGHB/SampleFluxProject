
namespace CBS.AccountManagement.Data
{
    public class FinancialReportConfigurations
    {
        public List<BalanceSheetAssetLiabilitiesDto> BalanceSheetLiabilities { get; set; }
        public List<BalanceSheetAssetDto> BalanceSheetAssets { get; set; }
        public List<IncomeDto> Incomes { get; set; }
        public List<ExpenseDto> Expenses { get; set; }
    }
}