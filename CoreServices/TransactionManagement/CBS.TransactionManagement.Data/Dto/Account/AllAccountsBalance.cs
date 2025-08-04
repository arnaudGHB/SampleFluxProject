using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Dto
{
    public class AllAccountsBalanceDto
    {
        public List<AccountDto> accounts { get; set; }=new List<AccountDto>();
        public string TotalBalance { get; set; }
    }
}