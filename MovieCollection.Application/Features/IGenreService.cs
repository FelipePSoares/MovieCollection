using System.Collections.Generic;
using System.Threading.Tasks;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Domain;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features
{
    public interface IGenreService
    {
        Task<AppResponse<List<GenreResponse>>> GetAllAsync();
        Task<AppResponse<List<Genre>>> RegisterAsync(List<Genre> genres);
    }
}
