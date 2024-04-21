using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
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
        Task<AppResponse<List<UserProfileResponse>>> GetAllUsersAsync(ClaimsPrincipal user, Paging paging);
        Task<AppResponse<UserProfileResponse>> AddMovieToCollectionAsync(ClaimsPrincipal userLogged, Guid movieId);
        Task<AppResponse<UserProfileResponse>> RemoveMovieFromCollectionAsync(ClaimsPrincipal userLogged, Guid movieId);
    }
}
