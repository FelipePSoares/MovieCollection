using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.AccessControl.Mappers;
using MovieCollection.Domain;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.Authentication;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features.AccessControl
{
    public class UserService : IUserService
    {
        private readonly string tokenProvider = "REFRESHTOKENPROVIDER";
        private readonly string tokenPurpose = "RefreshToken";
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly TokenSettings tokenSettings;
        private readonly IUnitOfWork unitOfWork;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenSettings tokenSettings,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenSettings = tokenSettings;
            this.unitOfWork = unitOfWork;
        }

        public async Task<AppResponse<UserLoginResponse>> UserLoginAsync(UserLoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return AppResponse<UserLoginResponse>.Error(nameof(request.Email), ValidationMessages.EmailNotFound);

            if (!user.Enabled)
                return AppResponse<UserLoginResponse>.Error(MessageKey.Blocked, string.Empty);

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded)
            {
                var token = await GenerateUserToken(user);
                return AppResponse<UserLoginResponse>.Success(token);
            }
            return AppResponse<UserLoginResponse>.Error(nameof(request.Password), result.ToString());
        }

        public async Task<AppResponse> UserLogoutAsync(ClaimsPrincipal user)
        {
            if (user.Identity?.IsAuthenticated ?? false)
            {
                var userId = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return AppResponse.Success();

                var appUser = await this.userManager.FindByIdAsync(userId);
                if (appUser != null)
                    await LogoutAsync(appUser);
            }
            return AppResponse.Success();
        }

        public async Task<AppResponse<UserRefreshTokenResponse>> UserRefreshTokenAsync(UserRefreshTokenRequest request)
        {
            var principal = TokenUtil.GetPrincipalFromExpiredToken(this.tokenSettings, request.AccessToken);
            if (principal == null || principal.FindFirst(ClaimTypes.NameIdentifier)?.Value == null)
                return AppResponse<UserRefreshTokenResponse>.Error(MessageKey.NotFound, ValidationMessages.UserNotFound);

            var user = await this.userManager.FindByIdAsync(principal.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (user == null)
                return AppResponse<UserRefreshTokenResponse>.Error(MessageKey.NotFound, ValidationMessages.UserNotFound);

            if (!user.Enabled)
                return AppResponse<UserRefreshTokenResponse>.Error(MessageKey.Blocked, string.Empty);

            if (!await this.userManager.VerifyUserTokenAsync(user, this.tokenProvider, this.tokenPurpose, request.RefreshToken))
                return AppResponse<UserRefreshTokenResponse>.Error(nameof(request.RefreshToken), ValidationMessages.RefreshTokenExpired);

            var token = await GenerateRefreshToken(user);

            return AppResponse<UserRefreshTokenResponse>.Success(token);
        }

        public async Task<AppResponse<UserProfileResponse>> UserRegisterAsync(UserRegisterRequest request)
        {
            var user = request.FromDTO();

            var result = await this.userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
                return AppResponse<UserProfileResponse>.Success(user.ToUserProfileResponse());
            else
                return AppResponse<UserProfileResponse>.Error(GetRegisterErrors(result));
        }

        public async Task<AppResponse<UserProfileResponse>> GetUserByIdAsync(Guid id)
        {
            var user = await this.userManager.Users
                .Include(user => user.MovieCollection)
                    .ThenInclude(movie => movie.Genres)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
                return AppResponse<UserProfileResponse>.Error(MessageKey.NotFound, ValidationMessages.UserNotFound);

            return AppResponse<UserProfileResponse>.Success(user.ToUserProfileResponse());
        }

        public async Task<AppResponse<UserProfileResponse>> SetUserNameAsync(ClaimsPrincipal userLogged, UserSetNameRequest userDto)
        {
            var userId = userLogged.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
                return AppResponse<UserProfileResponse>.Error(MessageKey.NotFound, ValidationMessages.UserNotFound);

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.HasIncompletedInformation = false;

            var result = await this.userManager.UpdateAsync(user);

            if (result.Succeeded)
                return AppResponse<UserProfileResponse>.Success(user.ToUserProfileResponse());

            return AppResponse<UserProfileResponse>.Error(GetRegisterErrors(result));
        }

        public async Task<AppResponse<UserProfileResponse>> UpdateMovieCollectionAsync(ClaimsPrincipal userLogged, JsonPatchDocument<UserMovieCollection> userMovieCollection)
        {
            var userId = new Guid(userLogged.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var user = await this.userManager.Users.Include(u => u.MovieCollection).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return AppResponse<UserProfileResponse>.Error(MessageKey.NotFound, ValidationMessages.UserNotFound);

            var userRequest = user.ToUserMovieCollection();

            userMovieCollection.ApplyTo(userRequest);
            user.MovieCollection = await this.unitOfWork.MovieRepository.Trackable()
                .Where(movie => userRequest.MovieCollection.Select(m => m.Id).Contains(movie.Id)).ToListAsync();

            var result = await this.userManager.UpdateAsync(user);

            if (result.Succeeded)
                return AppResponse<UserProfileResponse>.Success(user.ToUserProfileResponse());

            return AppResponse<UserProfileResponse>.Error(GetRegisterErrors(result));
        }

        public async Task<AppResponse> RemoveUserAsync(ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return AppResponse.Success();

            var appUser = await this.userManager.FindByIdAsync(userId);
            if (appUser == null)
                return AppResponse.Success();

            await this.LogoutAsync(appUser);
            var result = await this.userManager.DeleteAsync(appUser);

            if (result.Succeeded)
                return AppResponse.Success();

            return AppResponse.Error(GetRegisterErrors(result));
        }

        public async Task<AppResponse> BlockUserAsync(Guid userId)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return AppResponse<UserProfileResponse>.Error(MessageKey.NotFound, ValidationMessages.UserNotFound);

            user.Enabled = false;

            var result = await this.userManager.UpdateAsync(user);
            await this.LogoutAsync(user);

            if (result.Succeeded)
                return AppResponse.Success();

            return AppResponse.Error(GetRegisterErrors(result));
        }

        private static List<AppMessage> GetRegisterErrors(IdentityResult result) 
            => result.Errors.Select(error => new AppMessage(error.Code, error.Description)).ToList();

        private async Task<UserLoginResponse> GenerateUserToken(User user)
        {
            var result = await this.GenerateTokenAsync(user);

            return new UserLoginResponse(result.AccessToken, result.RefreshToken);
        }

        private async Task<UserRefreshTokenResponse> GenerateRefreshToken(User user)
        {
            var result = await this.GenerateTokenAsync(user);

            return new UserRefreshTokenResponse(result.AccessToken, result.RefreshToken);
        }

        private async Task<(string AccessToken, string RefreshToken)> GenerateTokenAsync(User user)
        {
            var userRoles = await this.userManager.GetRolesAsync(user);
            var claims = userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)).ToList();

            var token = TokenUtil.GetToken(tokenSettings, user, claims);

            await userManager.RemoveAuthenticationTokenAsync(user, this.tokenProvider, this.tokenPurpose);
            var refreshToken = await userManager.GenerateUserTokenAsync(user, this.tokenProvider, this.tokenPurpose);
            await userManager.SetAuthenticationTokenAsync(user, this.tokenProvider, this.tokenPurpose, refreshToken);

            return (AccessToken: token, RefreshToken: refreshToken);
        }

        private async Task<IdentityResult> LogoutAsync(User appUser) 
            => await userManager.UpdateSecurityStampAsync(appUser);
    }
}
