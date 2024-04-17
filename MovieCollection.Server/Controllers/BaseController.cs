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

            return BadRequest(appResponse.Messages);
        }

        protected IActionResult ValidateResponse(AppResponse appResponse, HttpStatusCode successStatusCode)
        {
            if (appResponse.IsSucceed)
                return StatusCode((int)successStatusCode);

            return BadRequest(appResponse.Messages);
        }
    }
}
