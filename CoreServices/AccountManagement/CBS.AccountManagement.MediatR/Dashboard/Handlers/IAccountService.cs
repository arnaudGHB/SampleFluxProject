using CBS.AccountManagement.MediatR.Handlers;

public interface IAccountService
{
    Dictionary<string, AccountTypeDefinition> _accounts { get; set; }
    AccountTypeDefinition GetAccountByKey(string key);
    AccountTypeDefinition GetAccountByCode(int code);
}