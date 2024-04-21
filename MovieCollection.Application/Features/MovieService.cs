using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Domain;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Features
{
    public class MovieService(IUnitOfWork unitOfWork, IGenreService genreService) : IMovieService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IGenreService genreService = genreService;

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

            return await InsertOrUpdateAsync(movie);
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
                .Include(movie => movie.Genres)
                .Where(movie => string.IsNullOrEmpty(filter.Title) ? true : movie.Title.Contains(filter.Title, StringComparison.InvariantCultureIgnoreCase))
                .Where(movie => !filter.Genres.Any() ? true : movie.Genres.Any(genre => filter.Genres.Contains(genre.Id)))
                .Where(movie => !filter.ReleaseYearStart.HasValue && !filter.ReleaseYearEnd.HasValue ? true : filter.ReleaseYearStart <= movie.ReleaseYear && movie.ReleaseYear <= filter.ReleaseYearEnd)
                .OrderBy(m => m.Title)
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

            return await InsertOrUpdateAsync(movie);
        }

        private async Task<AppResponse<MovieResponse>> InsertOrUpdateAsync(Movie movie)
        {
            var validationResult = movie.IsValid();

            if (!validationResult.Succeeded)
                return AppResponse<MovieResponse>.Error(validationResult.Messages);

            var registerGenresResult = await this.genreService.RegisterAsync(movie.Genres);

            if (registerGenresResult.Succeeded)
            {
                if (await this.unitOfWork.MovieRepository.NoTrackable().AnyAsync(m => m.Title == movie.Title))
                    return AppResponse<MovieResponse>.Error(nameof(movie.Title), ValidationMessages.MovieTitleAlreadyExists);

                movie.Genres = registerGenresResult.Data;
                var result = this.unitOfWork.MovieRepository.InsertOrUpdate(movie);

                if (result.Succeeded)
                {
                    await this.unitOfWork.CommitAsync();
                    return AppResponse<MovieResponse>.Success(result.Data.ToMovieResponse());
                }

                return AppResponse<MovieResponse>.Error(result.Messages);
            }

            return AppResponse<MovieResponse>.Error(registerGenresResult.Messages);
        }
    }
}
