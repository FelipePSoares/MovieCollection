using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Application.Features.AccessControl;
using MovieCollection.Application.Features.AccessControl.DTOs;

namespace MovieCollection.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController(IUserService userService) : BaseController
    {
        private readonly IUserService userService = userService;

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterRequest req)
        {
            var result = await userService.UserRegisterAsync(req);

            return ValidateResponse(result, HttpStatusCode.Created);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginRequest req)
        {
            var result = await userService.UserLoginAsync(req);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(UserRefreshTokenRequest req)
        {
            var result = await userService.UserRefreshTokenAsync(req);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var result = await userService.UserLogoutAsync(User);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        public string Profile()
        {
            return User.FindFirst("UserName")?.Value ?? "";
        }
    }
}
