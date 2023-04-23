using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebProject.Data;

namespace WebProject.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
    private readonly AppDbContext appDbContext;
    private  DbSet<T> dbSet;
    public Repository(AppDbContext context)
    {
        appDbContext = context;
        dbSet = appDbContext.Set<T>();
    }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public void delete(T entity)
        {
            dbSet.Remove(entity);
        }
        public void deleteRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
        public IEnumerable<T> GetAll(Expression<Func<T,bool>>? predicate = null, string? includeProperties=null)
        {
            IQueryable<T> queury = dbSet;
            if(predicate != null)
            {
                queury = queury.Where(predicate);
            }
            if(includeProperties != null)
            {
                foreach(var item in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    queury = queury.Include(item);
                }
            }
            return queury.ToList();
        }
        public T GetT(Expression<Func<T,bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(predicate);
            if(includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefault();
        }
    }
}
