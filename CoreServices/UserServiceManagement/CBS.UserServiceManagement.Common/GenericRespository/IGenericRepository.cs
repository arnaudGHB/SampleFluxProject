// Source: Votre code source exact.
using System.Linq.Expressions;

namespace CBS.UserServiceManagement.Common
{
    public interface IGenericRepository<TC> where TC : class
    {
        IQueryable<TC> All { get; }
        void Attach(TC entity);
        IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindByInclude(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindBy(Expression<Func<TC, bool>> predicate);
        TC Find(string id);
        Task<TC> FindAsync(string id);
        void Add(TC entity);
        void Update(TC entity);
        void UpdateInCasecade(TC entity);
        void UpdateRange(List<TC> entities);
        void Delete(string id);
        void Delete(TC entity);
        void Remove(TC entity);
        void InsertUpdateGraph(TC entity);
        void RemoveRange(IEnumerable<TC> lstEntities);
        void AddRange(IEnumerable<TC> lstEntities);
        Task SaveChangesAsync();
    }
}