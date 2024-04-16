using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MovieCollection.Application.Features.AccessControl;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure.Authentication;

namespace MovieCollection.Application.Tests.Features.AccessControl
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> userManagerMock;
        private readonly Mock<SignInManager<User>> signInManagerMock;
        private readonly TokenSettings tokenSettings;
        private readonly UserService userService;

        public UserServiceTests()
        {
            var store = new Mock<IUserStore<User>>();
            this.userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            this.signInManagerMock = new Mock<SignInManager<User>>(this.userManagerMock.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null, null, null);
            this.tokenSettings = new TokenSettings();

            this.userService = new UserService(this.userManagerMock.Object, this.signInManagerMock.Object, this.tokenSettings);
        }

        [Fact]
        public async Task UserRegisterAsync_SucessRegistration_ShouldReturnIsSucceedTrue()
        {
            // Arrange
            var user = new UserRegisterRequest();

            this.userManagerMock.Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var response = await this.userService.UserRegisterAsync(user);

            // Assert
            response.IsSucceed.Should().BeTrue();
            response.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task UserRegisterAsync_FailedRegistration_ShouldReturnIsSucceedFalseAndTheMessage()
        {
            // Arrange
            var user = new UserRegisterRequest();

            var error = new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Passwords must be at least 8 characters."
            };

            var error2 = new IdentityError
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Passwords must have at least one non alphanumeric character."
            };

            this.userManagerMock.Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(error, error2));

            // Act
            var response = await this.userService.UserRegisterAsync(user);

            // Assert
            response.IsSucceed.Should().BeFalse();
            response.Messages.Should().Contain(error.Code, error.Description);
            response.Messages.Should().Contain(error2.Code, error2.Description);
        }
    }
}
