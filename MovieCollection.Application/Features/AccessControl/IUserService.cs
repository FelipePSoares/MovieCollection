using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features.AccessControl
{
    public interface IUserService
    {
        Task<AppResponse<UserLoginResponse>> UserLoginAsync(UserLoginRequest req);
        Task<AppResponse> UserLogoutAsync(ClaimsPrincipal user);
        Task<AppResponse<UserRefreshTokenResponse>> UserRefreshTokenAsync(UserRefreshTokenRequest req);
        Task<AppResponse<UserProfileResponse>> UserRegisterAsync(UserRegisterRequest req);
        Task<AppResponse<UserProfileResponse>> GetUserByIdAsync(Guid id);
        Task<AppResponse<UserProfileResponse>> SetUserNameAsync(ClaimsPrincipal userLogged, UserSetNameRequest userDto);
        Task<AppResponse> RemoveUserAsync(ClaimsPrincipal user);
        Task<AppResponse> BlockUserAsync(Guid userId);
    }
}
