using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Domain;

namespace MovieCollection.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool disposed = false;
        private readonly MovieCollectionDatabaseContext context;
        private readonly Lazy<IGenericRepository<Movie>> movieRepository;
        private readonly Lazy<IGenericRepository<Genre>> genreRepository;

        public UnitOfWork(MovieCollectionDatabaseContext dbContext, ILogger<UnitOfWork> logger)
        {
            this.context = dbContext;
            this.movieRepository = new Lazy<IGenericRepository<Movie>>(() => new GenericRepository<Movie>(this.context, logger));
            this.genreRepository = new Lazy<IGenericRepository<Genre>>(() => new GenericRepository<Genre>(this.context, logger));
        }

        public IGenericRepository<Movie> MovieRepository => this.movieRepository.Value;
        public IGenericRepository<Genre> GenreRepository => this.genreRepository.Value;

        public Task CommitAsync()
        {
            var currentDateTime = DateTime.Now;
            var entries = this.context.ChangeTracker.Entries();

            // get a list of all Modified entries which implement the BaseEntity
            var updatedEntries = entries.Where(e => e.Entity is BaseEntity)
                    .Where(e => e.State == EntityState.Modified)
                    .ToList();

            updatedEntries.ForEach(e =>
            {
                e.Property("CreatedDate").IsModified = false;
                ((BaseEntity)e.Entity).ModifiedAt = currentDateTime;
            });

            // get a list of all Added entries which implement the BaseEntity
            var addedEntries = entries.Where(e => e.Entity is BaseEntity)
                    .Where(e => e.State == EntityState.Added)
                    .ToList();

            addedEntries.ForEach(e =>
            {
                ((BaseEntity)e.Entity).CreatedDate = currentDateTime;
                ((BaseEntity)e.Entity).ModifiedAt = currentDateTime;
            });

            return this.context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
