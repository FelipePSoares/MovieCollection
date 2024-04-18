using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features
{
    public class MovieService(IUnitOfWork unitOfWork) : IMovieService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<AppResponse<MovieResponse>> GetByIdAsync(Guid movieId)
        {
            var movie = await unitOfWork.MovieRepository.NoTrackable().Include(movie => movie.Genres).FirstOrDefaultAsync(movie => movie.Id == movieId);

            if (movie == null)
                return AppResponse<MovieResponse>.Error(MessageKey.NotFound, ValidationMessages.MovieNotFound);

            return AppResponse<MovieResponse>.Success(movie.ToMovieResponse());
        }

        public Task<AppResponse<MovieResponse>> RegisterAsync(MovieRegisterRequest req)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse> RemoveAsync(Guid movieId)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<List<MovieResponse>>> SearchAsync(MovieFilters filter)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<MovieResponse>> UpdateAsync(Guid movieId, JsonPatchDocument<MovieRegisterRequest> movieDto)
        {
            throw new NotImplementedException();
        }
    }
}
