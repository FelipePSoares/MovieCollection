using AutoFixture;

namespace MovieCollection.Common.Tests
{
    public class BaseTests
    {
        protected Fixture fixture;

        public BaseTests()
        {
            this.fixture = new Fixture();
        }
    }
}
