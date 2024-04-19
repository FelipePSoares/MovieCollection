using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features
{
    public interface IMovieService
    {
        Task<AppResponse<List<MovieResponse>>> SearchAsync(MovieFilters filter);
        Task<AppResponse<MovieResponse>> GetByIdAsync(Guid movieId);
        Task<AppResponse<MovieResponse>> RegisterAsync(MovieRequest req);
        Task<AppResponse<MovieResponse>> UpdateAsync(Guid movieId, JsonPatchDocument<MovieRequest> movieDto);
        Task<AppResponse> RemoveAsync(Guid movieId);
    }
}
