using System.Net;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Application.Features;
using MovieCollection.Application.Features.DTOs;

namespace MovieCollection.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController(IGenreService genreService) : BaseController
    {
        private readonly IGenreService genreService = genreService;

        [HttpGet]
        [ProducesResponseType(typeof(List<GenreResponse>), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAll()
        {
            var result = await this.genreService.GetAllAsync();

            return ValidateResponse(result, HttpStatusCode.OK);
        }
    }
}
