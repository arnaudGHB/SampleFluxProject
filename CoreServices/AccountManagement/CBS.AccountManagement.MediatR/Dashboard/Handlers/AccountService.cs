using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CBS.AccountManagement.MediatR.Handlers;
using Microsoft.AspNetCore.Hosting;

public class AccountService : IAccountService
{
    private readonly IWebHostEnvironment _env;
    public Dictionary<string, AccountTypeDefinition> _accounts { get; set; }

    public AccountService(IWebHostEnvironment env)
    {
        _env = env;
        _accounts= LoadAccounts();
    }

    private Dictionary<string, AccountTypeDefinition> LoadAccounts()
    {
        string filePath = Path.Combine(_env.ContentRootPath, "Dashboard.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
           return JsonSerializer.Deserialize<Dictionary<string, AccountTypeDefinition>>(jsonData);
        }
        else
        {
            return new Dictionary<string, AccountTypeDefinition>();
        }
    }
    public AccountTypeDefinition GetAccountByKey(string key)
    {
        if (_accounts.TryGetValue(key, out var account))
        {
            return account;
        }
        return null; // Return null if the key is not found
    }
    public AccountTypeDefinition GetAccountByCode(int code)
    {
        foreach (var account in _accounts.Values)
        {
            if (account.Code == code || (account.HasArray && account.CodeArray.Contains(code)))
            {
                return account;
            }
        }
        return null; // Return null if not found
    }
}
