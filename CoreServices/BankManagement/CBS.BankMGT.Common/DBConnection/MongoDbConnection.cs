using MongoDB.Driver;
using System;

namespace CBS.BankMGT.Common.DBConnection
{
 

    public interface IMongoDbConnection
    {
        IMongoDatabase Database { get; }
    }

    public class MongoDbConnection : IMongoDbConnection, IDisposable
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private bool _disposed = false;

        public MongoDbConnection(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName));

            // Initialize MongoDB Client with the provided connection string
            _mongoClient = new MongoClient(connectionString);

            // Get the MongoDB database
            _mongoDatabase = _mongoClient.GetDatabase(databaseName);
        }
        //
        // Expose the database object for repository interaction
        public IMongoDatabase Database => _mongoDatabase;

        // Dispose method for releasing resources
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                // In MongoDB, there are no explicit database connections to close.
                // However, if using specific resources, you could handle cleanup here.
            }
        }
    }

}
