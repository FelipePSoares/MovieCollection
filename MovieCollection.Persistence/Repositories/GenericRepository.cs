using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Domain;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> dbSet;
        private readonly ILogger logger;

        public GenericRepository(DbContext context, ILogger logger)
        {
            this.dbSet = context.Set<T>();
            this.logger = logger;
        }

        public IQueryable<T> Trackable() => this.dbSet.AsQueryable();

        public IQueryable<T> NoTrackable() => this.dbSet.AsNoTracking();

        public AppResponse<T> InsertOrUpdate(T entity)
        {
            try
            {
                var result = this.dbSet.Update(entity);

                return AppResponse<T>.Success(result.Entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on try to insert or update {typeof(T)}.");
                return AppResponse<T>.Error();
            }
        }

        public AppResponse Delete(T entity)
        {
            try
            {
                var result = this.dbSet.Remove(entity);

                return AppResponse.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on try to remove {typeof(T)}.");
                return AppResponse.Error();
            }
        }
    }
}
