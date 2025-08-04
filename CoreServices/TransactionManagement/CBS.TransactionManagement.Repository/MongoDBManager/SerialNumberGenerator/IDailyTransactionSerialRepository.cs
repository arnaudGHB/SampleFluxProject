using CBS.TransactionManagement.Data.Entity.MongoDBObjects.DailySerialNumber;
using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator
{
    public interface IDailyTransactionSerialRepository
    {
        Task MarkTransactionAsSuccessful(string transactionCode);
        Task<DailyTransactionSerial> ReserveTransactionCode(string branchCode, OperationPrefix operationPrefix, TransactionType operationType, bool isInterBranch);
        Task RevertTransactionCode(string transactionCode);
    }
}