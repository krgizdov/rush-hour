namespace RushHour.Domain.Repositories
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> GetAllPaginated(int page, int size);

        Task<TEntity> GetEntityAsync(Expression<Func<TEntity, bool>> filter);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task<int> SaveChangesAsync();
    }
}
