using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.AccessControl.Mappers;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.Authentication;

namespace MovieCollection.Application.Features.AccessControl
{
    public class UserService : IUserService
    {
        private readonly string tokenProvider = "REFRESHTOKENPROVIDER";
        private readonly string purpose = "RefreshToken";
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly TokenSettings tokenSettings;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenSettings tokenSettings)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenSettings = tokenSettings;
        }

        public async Task<AppResponse<UserLoginResponse>> UserLoginAsync(UserLoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return AppResponse<UserLoginResponse>.Error(nameof(request.Email), ValidationMessages.EmailNotFound);
            else
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    var token = await GenerateUserToken(user);
                    return AppResponse<UserLoginResponse>.Success(token);
                }
                else
                    return AppResponse<UserLoginResponse>.Error(nameof(request.Password), result.ToString());
            }
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
                    await userManager.UpdateSecurityStampAsync(appUser);
            }
            return AppResponse.Success();
        }

        public async Task<AppResponse<UserRefreshTokenResponse>> UserRefreshTokenAsync(UserRefreshTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AppResponse> UserRegisterAsync(UserRegisterRequest request)
        {
            var user = request.FromDTO();

            var result = await this.userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
                return AppResponse.Success();
            else
                return AppResponse.Error(GetRegisterErrors(result));
        }

        private Dictionary<string, string> GetRegisterErrors(IdentityResult result)
        {
            var errorDictionary = new Dictionary<string, string>(1);

            foreach (var error in result.Errors)
            {
                if (!errorDictionary.ContainsKey(error.Code))
                {
                    errorDictionary[error.Code] = error.Description;
                }   
            }

            return errorDictionary;
        }

        private async Task<UserLoginResponse> GenerateUserToken(User user)
        {
            var claims = await this.userManager.GetClaimsAsync(user);
            
            var token = TokenUtil.GetToken(tokenSettings, user, claims.ToList());

            await userManager.RemoveAuthenticationTokenAsync(user, this.tokenProvider, this.purpose);
            var refreshToken = await userManager.GenerateUserTokenAsync(user, this.tokenProvider, this.purpose);
            await userManager.SetAuthenticationTokenAsync(user, this.tokenProvider, this.purpose, refreshToken);
            return new UserLoginResponse() { AccessToken = token, RefreshToken = refreshToken };
        }
    }
}
