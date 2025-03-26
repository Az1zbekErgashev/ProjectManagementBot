using System.Linq.Expressions;
using ManagementBot.Helpers;

namespace ManagementBot.Interfaces
{
    public interface IGenericRepository<T> where T : Auditable
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null, string[] includes = null, bool isTracking = true);
        ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, bool isTracking = true, string[] includes = null);
        ValueTask<T> CreateAsync(T entity);
        T UpdateAsync(T entity);
        ValueTask<bool> DeleteAsync(int id);
        ValueTask SaveChangeAsync();
    }
}
