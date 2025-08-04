using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Dto
{
    public class AccountBalanceDto
    {
        public string AccountNumber { get; set; }
        public string Balance { get; set; }
        public string AccountName { get; set; }
    }
    public class AccountBalanceThirdPartyDto
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; } = 0;

    }
}