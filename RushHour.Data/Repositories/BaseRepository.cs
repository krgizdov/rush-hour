namespace RushHour.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using RushHour.Domain.Repositories;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAllPaginated(int page, int size)
        {
            return _dbSet.Skip((page - 1) * size).Take(size);
        }

        public virtual Task<TEntity> GetEntityAsync(Expression<Func<TEntity, bool>> filter)
        {
            return _dbSet.FirstOrDefaultAsync(filter);
        }

        public Task AddAsync(TEntity entity)
        {
            return _dbSet.AddAsync(entity).AsTask();
        }

        public void Update(TEntity entity)
        {
             _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
