using System.Linq.Expressions;

namespace WebProject.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? predicate=null,string? includeProperties=null);
        T GetT(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        void Add(T entity);
        void delete(T entity);
        void deleteRange(IEnumerable<T> entity);
    }
}
