using AutoFixture;
using FluentAssertions;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.AccessControl.Mappers;
using MovieCollection.Common.Tests;

namespace MovieCollection.Application.Tests.Features.AccessControl.Mappers
{
    public class UserRegisterMapTests : BaseTests
    {
        [Fact]
        public void FromDTO_MapData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var userRegister = this.fixture.Create<UserRegisterRequest>();

            // Act
            var user = userRegister.FromDTO();

            // Assert
            user.UserName.Should().Be(userRegister.Email);
            user.Email.Should().Be(userRegister.Email);
        }
    }
}
