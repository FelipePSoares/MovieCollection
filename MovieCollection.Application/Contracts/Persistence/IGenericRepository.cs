using System.Linq;
using MovieCollection.Domain;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> Trackable();
        IQueryable<T> NoTrackable();
        AppResponse<T> InsertOrUpdate(T entity);
        AppResponse Delete(T entity);
    }
}
