namespace CBS.AccountManagement.MediatR
{
    public class DataAccount
    {
        public bool IsRootAccount { get; set; }
        public Data.Account Account  { get; set; }
        public DataAccount( Data.Account account,bool isRoot=false)
        {
            IsRootAccount= isRoot;
            Account=account;
        }
    }
}