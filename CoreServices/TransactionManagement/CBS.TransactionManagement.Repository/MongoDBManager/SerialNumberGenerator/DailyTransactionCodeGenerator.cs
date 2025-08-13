using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator
{
    public class DailyTransactionCodeGenerator : IDailyTransactionCodeGenerator
    {
        private readonly IDailyTransactionSerialRepository _serialRepository;

        public DailyTransactionCodeGenerator(IDailyTransactionSerialRepository serialRepository)
        {
            _serialRepository = serialRepository ?? throw new ArgumentNullException(nameof(serialRepository));
        }

        /// <summary>
        /// Reserves a transaction code safely in a concurrent environment.
        /// </summary>
        public async Task<string> ReserveTransactionCode(
            string branchCode, OperationPrefix operationPrefix, TransactionType operationType, bool isInterBranch)
        {
            var reservedSerial = await _serialRepository.ReserveTransactionCode(branchCode, operationPrefix, operationType, isInterBranch);

            string prefix = OperationPrefixMapper.GetPrefix(operationPrefix, isInterBranch);
            string currentDate = BaseUtilities.UtcNowToDoualaTime().ToString("yyyyMMdd");

            return $"{prefix}-{branchCode}-{currentDate}{reservedSerial.LastSerialNumber:D4}";
        }

        /// <summary>
        /// Marks a transaction code as successfully used.
        /// </summary>
        public async Task MarkTransactionAsSuccessful(string transactionCode)
        {
            await _serialRepository.MarkTransactionAsSuccessful(transactionCode);
        }

        /// <summary>
        /// Reverts a failed transaction, making the transaction code reusable.
        /// </summary>
        public async Task RevertTransactionCode(string transactionCode)
        {
            await _serialRepository.RevertTransactionCode(transactionCode);
        }
    }

}
