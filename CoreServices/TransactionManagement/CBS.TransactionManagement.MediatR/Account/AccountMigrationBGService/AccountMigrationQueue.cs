using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.AccountMigrationBGService
{
    /// <summary>
    /// Manages a queue of account migration requests, ensuring thread-safe enqueue and dequeue operations.
    /// </summary>
    public class AccountMigrationQueue
    {
        private readonly Queue<AccountMigrationCommand> _queue = new();
        private readonly SemaphoreSlim _queueLock = new(1, 1);

        /// <summary>
        /// Adds an account migration request to the queue in a thread-safe manner.
        /// </summary>
        /// <param name="request">The account migration command to enqueue.</param>
        public async Task EnqueueAsync(AccountMigrationCommand request)
        {
            await _queueLock.WaitAsync();
            try
            {
                _queue.Enqueue(request);
                string enqueueMessage = $"Account migration request enqueued. Queue size: {_queue.Count}";
                await BaseUtilities.LogAndAuditAsync(enqueueMessage, "AccountMigrationQueue", HttpStatusCodeEnum.OK, LogAction.AccountBalanceMigrationQueueEnqueue, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
            }
            finally
            {
                _queueLock.Release();
            }
        }

        /// <summary>
        /// Retrieves and removes the next account migration request from the queue in a thread-safe manner.
        /// </summary>
        /// <returns>The dequeued account migration command or null if the queue is empty.</returns>
        public async Task<AccountMigrationCommand?> DequeueAsync()
        {
            await _queueLock.WaitAsync();
            try
            {
                if (_queue.Count > 0)
                {
                    var request = _queue.Dequeue();
                    string dequeueMessage = $"Account migration request dequeued. Queue size: {_queue.Count}";
                    await BaseUtilities.LogAndAuditAsync(dequeueMessage, "AccountMigrationQueue", HttpStatusCodeEnum.OK, LogAction.AccountBalanceMigrationQueueDequeue, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                    return request;
                }
                return null;
            }
            finally
            {
                _queueLock.Release();
            }
        }

        /// <summary>
        /// Gets the current count of requests in the queue.
        /// </summary>
        /// <returns>The number of requests in the queue.</returns>
        public int GetQueueCount()
        {
            return _queue.Count;
        }
    }

}
