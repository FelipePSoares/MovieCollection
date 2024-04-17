using AutoFixture;
using FluentAssertions;
using MovieCollection.Application.Features.AccessControl.Mappers;
using MovieCollection.Common.Tests;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Application.Tests.Features.AccessControl.Mappers
{
    public class UserProfileMapTests : BaseTests
    {
        [Fact]
        public void ToUserProfileResponse_MapData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var user = this.fixture.Create<User>();

            // Act
            var userProfile = user.ToUserProfileResponse();

            // Assert
            userProfile.Id.Should().Be(user.Id);
            userProfile.FirstName.Should().Be(user.FirstName);
            userProfile.LastName.Should().Be(user.LastName);
            userProfile.Enabled.Should().Be(user.Enabled);
            userProfile.IsFirstLogin.Should().Be(user.IsFirstLogin);
            userProfile.MovieCollection.Should().HaveCount(user.MovieCollection.Count);
        }
    }
}
