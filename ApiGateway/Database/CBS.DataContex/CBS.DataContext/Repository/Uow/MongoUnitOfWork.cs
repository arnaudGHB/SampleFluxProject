using CBS.Gateway.DataContext.DBConnection;
using CBS.Gateway.DataContext.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Gateway.DataContext.Repository.Uow
{
    public class MongoUnitOfWork : IMongoUnitOfWork
    {
        private readonly IMongoDbConnection _mongoDbConnection;
        private readonly Dictionary<Type, object> _repositories;

        public MongoUnitOfWork(IMongoDbConnection mongoDbConnection)
        {
            _mongoDbConnection = mongoDbConnection;
            _repositories = new Dictionary<Type, object>();
        }

        public IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_repositories.ContainsKey(entityType))
            {
                var repository = new MongoGenericRepository<TEntity>(_mongoDbConnection);
                _repositories[entityType] = repository;
            }

            return (IMongoGenericRepository<TEntity>)_repositories[entityType];
        }

        public void Dispose()
        {
            // Dispose resources (if necessary)
        }
    }
}
