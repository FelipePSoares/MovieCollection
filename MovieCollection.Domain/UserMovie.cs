using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Domain
{
    public class UserMovie : BaseEntity
    {
        public User User { get; private set; } = new User();
        public Movie Movie { get; private set; } = new Movie();
    }
}
