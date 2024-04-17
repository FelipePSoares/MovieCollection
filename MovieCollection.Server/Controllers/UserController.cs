using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        [ProducesResponseType(typeof(UserProfileResponse), 201)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterRequest req)
        {
            var result = await userService.UserRegisterAsync(req);

            return ValidateResponse(result, HttpStatusCode.Created);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(UserLoginResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginRequest req)
        {
            var result = await userService.UserLoginAsync(req);

            if (!result.IsSucceed && result.Messages.ContainsKey("Blocked"))
                return Forbid();

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(UserRefreshTokenResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(UserRefreshTokenRequest req)
        {
            var result = await userService.UserRefreshTokenAsync(req);

            if (!result.IsSucceed && result.Messages.ContainsKey("Blocked"))
                return Forbid();

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Logout()
        {
            var result = await userService.UserLogoutAsync(User);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(UserProfileResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Profile()
        {
            var userId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            var result = await userService.GetUserByIdAsync(new Guid(userId));

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserProfileResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var result = await userService.GetUserByIdAsync(userId);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPut("[action]")]
        [ProducesResponseType(typeof(UserProfileResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SetUserName(UserSetNameRequest userDto)
        {
            var result = await userService.SetUserNameAsync(User, userDto);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RemoveUser()
        {
            var result = await userService.RemoveUserAsync(User);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Block(Guid userId)
        {
            var result = await userService.BlockUserAsync(userId);

            return ValidateResponse(result, HttpStatusCode.OK);
        }
    }
}
