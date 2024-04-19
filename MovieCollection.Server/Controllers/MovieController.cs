using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Application.Features;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController(IMovieService movieService) : BaseController
    {
        private readonly IMovieService movieService = movieService;

        [HttpGet]
        [ProducesResponseType(typeof(List<MovieResponse>), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Get([FromQuery] MovieFilters filter)
        {
            AppResponse<List<MovieResponse>> result = await this.movieService.SearchAsync(filter);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpGet("{movieId}")]
        [ProducesResponseType(typeof(MovieResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetById(Guid movieId)
        {
            var result = await this.movieService.GetByIdAsync(movieId);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MovieResponse), 201)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Register(MovieRequest req)
        {
            var result = await this.movieService.RegisterAsync(req);

            return ValidateResponse(result, HttpStatusCode.Created);
        }

        [HttpPatch("{movieId}")]
        [ProducesResponseType(typeof(MovieResponse), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Update(Guid movieId, [FromBody] JsonPatchDocument<MovieRequest> movieDto)
        {
            var result = await this.movieService.UpdateAsync(movieId, movieDto);

            return ValidateResponse(result, HttpStatusCode.OK);
        }

        [HttpDelete("{movieId}")]
        [ProducesResponseType(typeof(MovieResponse), 204)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteProjectAsync(Guid movieId)
        {
            var result = await this.movieService.RemoveAsync(movieId);

            return ValidateResponse(result, HttpStatusCode.NoContent);
        }
    }
}
