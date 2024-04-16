using Bogus;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Common.Tests.Fakers.AccessControl
{
    public class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            RuleFor(p => p.Id, f => f.Random.Guid());
            RuleFor(p => p.FirstName, f => f.Name.FirstName());
            RuleFor(p => p.LastName, f => f.Name.LastName());
        }
    }
}
