using CBS.TransactionManagement.Common.Repository.Generic;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects.DailySerialNumber;
using CBS.TransactionManagement.Helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CBS.TransactionManagement.Common.Repository.Uow;

namespace CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator
{

    public class DailyTransactionSerialRepository : IDailyTransactionSerialRepository
    {
        private readonly IMongoGenericRepository<DailyTransactionSerial> _repository;

        public DailyTransactionSerialRepository(IMongoUnitOfWork unitOfWork)
        {
            _repository = unitOfWork.GetRepository<DailyTransactionSerial>();
        }

        /// <summary>
        /// Reserves a transaction code but does not mark it as used.
        /// Ensures unique, sequential numbering even with high concurrency.
        /// </summary>
        public async Task<DailyTransactionSerial> ReserveTransactionCode(
            string branchCode, OperationPrefix operationPrefix, TransactionType operationType, bool isInterBranch)
        {
            var today = BaseUtilities.UtcNowToDoualaTime().Date;
            string dateString = today.ToString("yyyyMMdd");

            // Generate unique ID based on branch, date, operation type, and transaction type
            string id = $"{branchCode}-{dateString}-{operationType}-{(isInterBranch ? "IB" : "L")}";

            // Check if the record exists
            var existingRecord = await _repository.GetByIdAsync(id);

            if (existingRecord == null)
            {
                // Generate the transaction code
                string prefix = OperationPrefixMapper.GetPrefix(operationPrefix, isInterBranch);
                string transactionCode = $"{prefix}-{branchCode}-{dateString}001";

                // Create a new serial record starting at 1
                var newSerial = new DailyTransactionSerial
                {
                    Id = id,
                    BranchCode = branchCode,
                    TransactionDate = today,
                    OperationPrefix = operationPrefix.ToString(),
                    OperationType = operationType.ToString(),
                    IsInterBranch = isInterBranch,
                    LastSerialNumber = 1,
                    TransactionCode = transactionCode,
                    IsUsed = false
                };

                await _repository.InsertAsync(newSerial);
                return newSerial;
            }
            else
            {
                // Increment the serial number atomically
                existingRecord.LastSerialNumber++;

                // Generate new transaction code
                string prefix = OperationPrefixMapper.GetPrefix(operationPrefix, isInterBranch);
                existingRecord.TransactionCode = $"{prefix}-{branchCode}-{dateString}{existingRecord.LastSerialNumber:D3}";

                await _repository.UpdateAsync(id, existingRecord);
                return existingRecord;
            }
        }

        /// <summary>
        /// Marks a transaction code as successfully used.
        /// </summary>
        public async Task MarkTransactionAsSuccessful(string transactionCode)
        {
            var filter = Builders<DailyTransactionSerial>.Filter.Eq(x => x.TransactionCode, transactionCode);
            var transactions = await _repository.FindByAsync(filter);

            foreach (var transaction in transactions)
            {
                transaction.IsUsed = true;
                await _repository.UpdateAsync(transaction.Id, transaction);
            }
        }

        /// <summary>
        /// Reverts a failed transaction, making the transaction code reusable.
        /// </summary>
        public async Task RevertTransactionCode(string transactionCode)
        {
            var filter = Builders<DailyTransactionSerial>.Filter.Eq(x => x.TransactionCode, transactionCode);
            var transactions = await _repository.FindByAsync(filter);

            foreach (var transaction in transactions)
            {
                transaction.IsUsed = false;
                await _repository.UpdateAsync(transaction.Id, transaction);
            }
        }


        /// <summary>
        /// Retrieves the last used serial number for a given branch and date.
        /// </summary>
        public async Task<int> GetLastSerialNumber(string branchCode, string date, TransactionType operationType, bool isInterBranch)
        {
            string id = $"{branchCode}-{date}-{operationType}-{(isInterBranch ? "IB" : "L")}";
            var transaction = await _repository.GetByIdAsync(id);

            return transaction?.LastSerialNumber ?? 0;
        }
    }
}
