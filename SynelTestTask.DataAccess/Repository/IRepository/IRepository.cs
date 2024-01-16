using System.Linq.Expressions;

namespace SynelTestTask.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T Get(Expression<Func<T, bool>> filter);
    void Add(T enitity);
    void Remove(T enitity);
    void RemoveRange(IEnumerable<T> entities);
}