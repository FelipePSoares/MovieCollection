using System.Threading.Tasks;

namespace MovieCollection.Application.Contracts.Persistence
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
