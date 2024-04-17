using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Application.Features.AccessControl;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure;

namespace MovieCollection.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : BaseController
    {
        private readonly IUserService userService = userService;

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterRequest req)
        {
            var result = await userService.UserRegisterAsync(req);

            return ValidateResponse(result, HttpStatusCode.Created);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginRequest req)
        {
            var result = await userService.UserLoginAsync(req);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(UserRefreshTokenRequest req)
        {
            var result = await userService.UserRefreshTokenAsync(req);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            var result = await userService.UserLogoutAsync(User);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            var result = await userService.GetUserByIdAsync(new Guid(userId));

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Profile(Guid userId)
        {
            var result = await userService.GetUserByIdAsync(userId);

            return ValidateResponse(result, HttpStatusCode.OK);
        }
    }
}
