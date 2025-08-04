namespace CBS.AccountManagement.Data.Entity.Currency
{
    public class Currency
    {
        public string Id { get; set; }

        public string CurrencyCode { get; set; }= "XAF";
        public string CurrencyName { get; set; }
        public string CurrencyDescription { get; set; }
        public string CurrencyPrice { get; set; }
        public string CurrencyPriceType { get; set; }

        public static string GetCurrency()=> "XAF";
        
    }
}