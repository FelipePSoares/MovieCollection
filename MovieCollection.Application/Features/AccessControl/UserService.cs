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

        public async Task<AppResponse<UserLoginResponse>> UserLoginAsync(UserLoginRequest req)
        {
            throw new NotImplementedException();
        }

        public async Task<AppResponse> UserLogoutAsync(ClaimsPrincipal user)
        {
            throw new NotImplementedException(); 
        }

        public async Task<AppResponse<UserRefreshTokenResponse>> UserRefreshTokenAsync(UserRefreshTokenRequest req)
        {
            throw new NotImplementedException();
        }

        public async Task<AppResponse> UserRegisterAsync(UserRegisterRequest req)
        {
            var user = req.FromDTO();

            var result = await this.userManager.CreateAsync(user, req.Password);

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
