using System.Security.Claims;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MovieCollection.Application.Features.AccessControl;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Common.Tests;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.Authentication;
using MovieCollection.Common.Tests.Extensions;
using MovieCollection.Application.Contracts.Persistence;

namespace MovieCollection.Application.Tests.Features.AccessControl
{
    public class UserServiceTests : BaseTests
    {
        private readonly Mock<UserManager<User>> userManagerMock;
        private readonly Mock<SignInManager<User>> signInManagerMock;
        private readonly TokenSettings tokenSettings;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly IUserService userService;

        public UserServiceTests()
        {
            var store = new Mock<IUserStore<User>>();
            this.userManagerMock = new Mock<UserManager<User>>(store.Object, default!, default!, default!, default!, default!, default!, default!, default!);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            this.signInManagerMock = new Mock<SignInManager<User>>(this.userManagerMock.Object, contextAccessorMock.Object, claimsFactoryMock.Object, default!, default!, default!);
            this.tokenSettings = new TokenSettings()
            {
                SecretKey = Guid.NewGuid().ToString()
            };
            this.unitOfWork = new Mock<IUnitOfWork>();

            this.userService = new UserService(this.userManagerMock.Object, this.signInManagerMock.Object, this.tokenSettings, this.unitOfWork.Object);
        }

        [Fact]
        public async Task UserRegisterAsync_SucessRegistration_ShouldReturnSucceededTrue()
        {
            // Arrange
            var user = new UserRegisterRequest();

            this.userManagerMock.Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var response = await this.userService.UserRegisterAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task UserRegisterAsync_FailedRegistration_ShouldReturnSucceededFalseAndTheMessage()
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
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain(error.Code, error.Description);
            response.Messages.Should().Contain(error2.Code, error2.Description);
        }

        [Fact]
        public async Task UserLoginAsync_RightInformation_ShouldReturnSucceededTrueAndToken()
        {
            // Arrange
            var refreshToken = Guid.NewGuid().ToString();
            var user = new UserLoginRequest();

            this.userManagerMock.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.GetClaimsAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<Claim>());

            this.userManagerMock.Setup(userManager => userManager.GenerateUserTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(refreshToken);

            this.userManagerMock.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string>());

            this.signInManagerMock.Setup(signInManager => signInManager.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var response = await this.userService.UserLoginAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            response.Data.Should().NotBeNull();
            response.Data.AccessToken.Should().NotBeNull();
            response.Data.RefreshToken.Should().Be(refreshToken);
        }

        [Fact]
        public async Task UserLoginAsync_UserBlocked_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var refreshToken = Guid.NewGuid().ToString();
            var user = new UserLoginRequest();

            this.userManagerMock.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User()
                {
                    Enabled = false
                });

            // Act
            var response = await this.userService.UserLoginAsync(user);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().ContainKey("Blocked");
        }

        [Fact]
        public async Task UserLoginAsync_EmailNotExist_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var user = new UserLoginRequest();

            // Act
            var response = await this.userService.UserLoginAsync(user);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("Email", ValidationMessages.EmailNotFound);
        }

        [Fact]
        public async Task UserLoginAsync_WrongPassword_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var user = new UserLoginRequest();

            this.userManagerMock.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.signInManagerMock.Setup(signInManager => signInManager.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var response = await this.userService.UserLoginAsync(user);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("Password", "Failed");
        }

        [Fact]
        public async Task UserLogoutAsync_AlreadyLogout_ShouldReturnSucceededTrue()
        {
            // Arrange
            var user = new ClaimsPrincipal();

            // Act
            var response = await this.userService.UserLogoutAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task UserLogoutAsync_SucceedLogout_ShouldReturnSucceededTrue()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User())
                .Verifiable();

            this.userManagerMock.Setup(userManager => userManager.UpdateSecurityStampAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            // Act
            var response = await this.userService.UserLogoutAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            this.userManagerMock.Verify(userManager => userManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
            this.userManagerMock.Verify(userManager => userManager.UpdateSecurityStampAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UserLogoutAsync_LogoutWithError_ShouldReturnSucceededTrue()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .Verifiable();

            this.userManagerMock.Setup(userManager => userManager.UpdateSecurityStampAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            // Act
            var response = await this.userService.UserLogoutAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            this.userManagerMock.Verify(userManager => userManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
            this.userManagerMock.Verify(userManager => userManager.UpdateSecurityStampAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UserRefreshTokenAsync_SucceedRefreshToken_ShouldReturnSucceededTrue()
        {
            // Arrange
            var token = GenerateToken(this.tokenSettings);
            var userRefreshTokenRequest = new UserRefreshTokenRequest()
            {
                AccessToken = token,
            };

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.VerifyUserTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            this.userManagerMock.Setup(userManager => userManager.GetClaimsAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<Claim>());

            this.userManagerMock.Setup(userManager => userManager.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string>());

            // Act
            var response = await this.userService.UserRefreshTokenAsync(userRefreshTokenRequest);

            // Assert
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task UserRefreshTokenAsync_UserBlocked_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var token = GenerateToken(this.tokenSettings);
            var userRefreshTokenRequest = new UserRefreshTokenRequest()
            {
                AccessToken = token,
            };

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User()
                {
                    Enabled = false
                });

            // Act
            var response = await this.userService.UserRefreshTokenAsync(userRefreshTokenRequest);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().ContainKey("Blocked");
        }

        [Fact]
        public async Task UserRefreshTokenAsync_UserNotExist_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var token = GenerateToken(this.tokenSettings);
            var userRefreshTokenRequest = new UserRefreshTokenRequest()
            {
                AccessToken = token,
            };

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()));

            // Act
            var response = await this.userService.UserRefreshTokenAsync(userRefreshTokenRequest);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain(MessageKey.NotFound, ValidationMessages.UserNotFound);
        }

        [Fact]
        public async Task UserRefreshTokenAsync_InvalidRefreshToken_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var token = GenerateToken(this.tokenSettings);
            var userRefreshTokenRequest = new UserRefreshTokenRequest()
            {
                AccessToken = token,
            };


            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.VerifyUserTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var response = await this.userService.UserRefreshTokenAsync(userRefreshTokenRequest);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain(nameof(userRefreshTokenRequest.RefreshToken), ValidationMessages.RefreshTokenExpired);
        }

        [Fact]
        public async Task SetUserNameAsync_SucceedUpdate_ShouldReturnSucceededTrue()
        {
            // Arrange
            User expectedUser = new User();
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var userDto = Fixture.Create<UserSetNameRequest>();

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.UpdateAsync(It.IsAny<User>()))
                .Callback((User u) => expectedUser = u)
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var response = await this.userService.SetUserNameAsync(user, userDto);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            expectedUser.FirstName.Should().Be(userDto.FirstName);
            expectedUser.LastName.Should().Be(userDto.LastName);
            expectedUser.HasIncompletedInformation.Should().BeFalse();
        }

        [Fact]
        public async Task SetUserNameAsync_UserNotExist_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var userDto = Fixture.Create<UserSetNameRequest>();

            // Act
            var response = await this.userService.SetUserNameAsync(user, userDto);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain(MessageKey.NotFound, ValidationMessages.UserNotFound);
        }

        [Fact]
        public async Task RemoveUserAsync_SucceedRemoved_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var userDto = Fixture.Create<UserSetNameRequest>();

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var response = await this.userService.RemoveUserAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveUserAsync_ErrorOnRemove_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var userDto = Fixture.Create<UserSetNameRequest>();

            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()
                {
                    Code = "code",
                    Description = "description"
                }));

            // Act
            var response = await this.userService.RemoveUserAsync(user);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("code", "description");
        }

        [Fact]
        public async Task RemoveUserAsync_UserNotExist_ShouldReturnSucceededTrue()
        {
            // Arrange
            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);

            this.userManagerMock.Setup(userManager => userManager.DeleteAsync(It.IsAny<User>()))
                .Verifiable();

            // Act
            var response = await this.userService.RemoveUserAsync(user);

            // Assert
            response.Succeeded.Should().BeTrue();
            this.userManagerMock.Verify(userManager => userManager.DeleteAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task BlockUserAsync_SucceedBlocked_ShouldReturnSucceededTrue()
        {
            // Arrange
            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var response = await this.userService.BlockUserAsync(Guid.NewGuid());

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task BlockUserAsync_UserNotExist_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            // Act
            var response = await this.userService.BlockUserAsync(Guid.NewGuid());

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain(MessageKey.NotFound, ValidationMessages.UserNotFound);
        }

        [Fact]
        public async Task BlockUserAsync_FailureOnBlock_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            this.userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManagerMock.Setup(userManager => userManager.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()
                {
                    Code = "code",
                    Description = "description"
                }));

            // Act
            var response = await this.userService.BlockUserAsync(Guid.NewGuid());

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("code", "description");
        }

        private string GenerateToken(TokenSettings tokenSettings)
        {
            var user = new User()
            {
                Id = Guid.NewGuid()
            };
            var roleClaims = new List<Claim>();

            var token = TokenUtil.GetToken(tokenSettings, user, roleClaims);
            return token;
        }
    }
}
