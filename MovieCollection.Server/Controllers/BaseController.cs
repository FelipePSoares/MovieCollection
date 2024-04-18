using System.Net;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Infrastructure;

namespace MovieCollection.Server.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult ValidateResponse<T>(AppResponse<T> appResponse, HttpStatusCode successStatusCode)
        {
            if (appResponse.IsSucceed)
                return StatusCode((int)successStatusCode, appResponse.Data);

            return ValidateResponse(appResponse);
        }

        protected IActionResult ValidateResponse(AppResponse appResponse, HttpStatusCode successStatusCode)
        {
            if (appResponse.IsSucceed)
                return StatusCode((int)successStatusCode);

            return ValidateResponse(appResponse);
        }

        private IActionResult ValidateResponse(AppResponse appResponse)
        {
            if (appResponse.Messages.ContainsKey(MessageKey.NotFound))
                return NotFound();

            if (appResponse.Messages.ContainsKey(MessageKey.Blocked))
                return Forbid();

            return BadRequest(appResponse.Messages);
        }
    }
}
