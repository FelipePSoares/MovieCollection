using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<AppResponse<MovieResponse>> RegisterAsync(MovieRequest req)
        {
            var movie = req.FromDTO();

            var validationResult = movie.IsValid();

            if (!validationResult.Succeeded)
                return AppResponse<MovieResponse>.Error(validationResult.Messages);

            var result = this.unitOfWork.MovieRepository.InsertOrUpdate(movie);
            
            if (result.Succeeded)
            {
                await this.unitOfWork.CommitAsync();
                return AppResponse<MovieResponse>.Success(result.Data.ToMovieResponse());
            }

            return AppResponse<MovieResponse>.Error(result.Messages);
        }

        public async Task<AppResponse> RemoveAsync(Guid movieId)
        {
            var movie = await unitOfWork.MovieRepository.Trackable().FirstOrDefaultAsync(movie => movie.Id == movieId);

            if (movie == null)
                return AppResponse.Success();

            var result = this.unitOfWork.MovieRepository.Delete(movie);

            if (result.Succeeded)
            {
                await this.unitOfWork.CommitAsync();
                return AppResponse.Success();
            }

            return result;
        }
        
        public async Task<AppResponse<List<MovieResponse>>> SearchAsync(MovieFilters filter)
        {
            var movies = await unitOfWork.MovieRepository
                .NoTrackable()
                .Where(movie => string.IsNullOrEmpty(filter.Title) ? true : movie.Title.Contains(filter.Title, StringComparison.InvariantCultureIgnoreCase))
                .Where(movie => !filter.Genres.Any() ? true : movie.Genres.Any(genre => filter.Genres.Contains(genre.Id)))
                .Where(movie => !filter.ReleaseYearStart.HasValue && !filter.ReleaseYearEnd.HasValue ? true : filter.ReleaseYearStart <= movie.ReleaseYear && movie.ReleaseYear <= filter.ReleaseYearEnd)
                .ToListAsync();

            return AppResponse<List<MovieResponse>>.Success(movies.ToMovieResponse());
        }

        public async Task<AppResponse<MovieResponse>> UpdateAsync(Guid movieId, JsonPatchDocument<MovieRequest> movieDto)
        {
            var movie = await unitOfWork.MovieRepository.NoTrackable().Include(movie => movie.Genres).FirstOrDefaultAsync(movie => movie.Id == movieId);

            if (movie == null)
                return AppResponse<MovieResponse>.Error(MessageKey.NotFound, ValidationMessages.MovieNotFound);

            var movieRequest = movie.ToMovieRequest();

            movieDto.ApplyTo(movieRequest);
            movie = movieRequest.FromDTO();
            movie.Id = movieId;

            var validationResult = movie.IsValid();

            if (!validationResult.Succeeded)
                return AppResponse<MovieResponse>.Error(validationResult.Messages);

            var result = unitOfWork.MovieRepository.InsertOrUpdate(movie);

            if (result.Succeeded)
            {
                await unitOfWork.CommitAsync();
                return AppResponse<MovieResponse>.Success(movie.ToMovieResponse());
            }

            return AppResponse<MovieResponse>.Error(result.Messages);
        }
    }
}
