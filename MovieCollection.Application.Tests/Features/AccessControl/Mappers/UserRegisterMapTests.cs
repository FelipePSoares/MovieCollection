using FluentAssertions;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.AccessControl.Mappers;

namespace MovieCollection.Application.Tests.Features.AccessControl.Mappers
{
    public class UserRegisterMapTests
    {
        [Fact]
        public void FromDTO_MapData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var userRegister = new UserRegisterRequest()
            {
                Email = "teste@teste.com"
            };

            // Act
            var user = userRegister.FromDTO();

            // Assert
            user.UserName.Should().Be("teste@teste.com");
            user.Email.Should().Be("teste@teste.com");
        }
    }
}
