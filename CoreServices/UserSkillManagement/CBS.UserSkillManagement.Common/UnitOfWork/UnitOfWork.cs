using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CBS.UserSkillManagement.Common
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private bool _disposed = false;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TContext Context => _context;

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            _repositories ??= new Dictionary<Type, object>();

            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T, TContext>(this);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
