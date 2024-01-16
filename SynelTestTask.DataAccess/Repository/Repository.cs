using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SynelTestTask.DataAccess.Data;
using SynelTestTask.DataAccess.Repository.IRepository;

namespace SynelTestTask.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext dbContext;
    internal DbSet<T> dbSet;

    public Repository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = dbContext.Set<T>();
    }

    public void Add(T enitity)
    {
        dbSet.Add(enitity);
    }

    public T Get(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        return query.FirstOrDefault();
    }

    public IEnumerable<T> GetAll()
    {
        IQueryable<T> query = dbSet;
        return query.ToList();
    }


    public void Remove(T enitity)
    {
        dbSet.Remove(enitity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }
}