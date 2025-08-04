using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator
{
    public interface IDailyTransactionCodeGenerator
    {
        Task MarkTransactionAsSuccessful(string transactionCode);
        Task<string> ReserveTransactionCode(string branchCode, OperationPrefix operationPrefix, TransactionType operationType, bool isInterBranch);
        Task RevertTransactionCode(string transactionCode);
    }
}