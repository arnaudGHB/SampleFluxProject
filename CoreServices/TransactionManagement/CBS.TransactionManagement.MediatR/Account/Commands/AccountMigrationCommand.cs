using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AccountMigrationCommand :  IRequest<ServiceResponse<bool>>
    {
        public string ProductId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public int NewAccountsCount { get; set; } = 0;
        public int ExistingAccountsCount { get; set; } = 0;
        public UserInfoToken? UserInfoToken { get; set; }
        public List<Data> Accounts { get; set; }
        public string? CorrelationId { get; internal set; }
        public AccountMigrationCommand()
        {
            CorrelationId=Guid.NewGuid().ToString();
        }
    }
    public class Data
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string BranchCode { get; set; }
        public decimal OpeningBalance { get; set; }
    }
   
}
