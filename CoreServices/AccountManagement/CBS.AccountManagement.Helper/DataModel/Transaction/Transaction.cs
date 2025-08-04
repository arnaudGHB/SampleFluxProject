public class AddCashCillingMovementCommand  
{
    public decimal Amount { get; set; }
    public DateTime AccountingDate { get; set; }
    public decimal ActualBalance { get; set; }
    public string AccountNumber { get; set; }
    public string CashInOrCashOut { get; set; }// CashIn Or CashOut
    public string? Note { get; set; }
    public CurrencyNotesRequest CurrencyNote { get; set; }
    public string BranchId { get; set; }
    public string BankId { get; set; }
    public string Reference { get; set; }
}

public class CurrencyNotesRequest
{
    public int Note10000 { get; set; }
    public int Note5000 { get; set; }
    public int Note2000 { get; set; }
    public int Note1000 { get; set; }
    public int Note500 { get; set; }
    public int Coin500 { get; set; }
    public int Coin100 { get; set; }
    public int Coin50 { get; set; }
    public int Coin25 { get; set; }
    public int Coin10 { get; set; }
    public int Coin5 { get; set; }
    public int Coin1 { get; set; }
}
