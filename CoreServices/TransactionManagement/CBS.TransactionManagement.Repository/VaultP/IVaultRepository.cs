using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository.VaultP
{
    public interface IVaultRepository : IGenericRepository<Vault>
    {
        void CashInByDenomination(decimal amount, CurrencyNotesRequest denominations, string branchId, string reference, string LastOperation, decimal cashInHand = 0, decimal cashinVault = 0, string InitializationNote = "N/A", bool isInternal=false);
        Task<bool> CashOutByDenominationAsync(decimal amount, CurrencyNotesRequest denominations, string branchId, string reference, string LastOperation, bool isInternal);
        void TransferCash(string fromVaultId, string toVaultId, decimal amount, CurrencyNotesRequest denominations, string reference,string LastOperation, bool isInternal);
        Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement);
        decimal GetVaultBalance(string branchId);
        void VerifySufficientFunds(decimal amount);
    }
}