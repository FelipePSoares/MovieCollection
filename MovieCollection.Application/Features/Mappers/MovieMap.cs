﻿using System.Collections.Generic;
using System.Linq;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Domain;

namespace MovieCollection.Application.Features.Mappers
{
    public static class MovieMap
    {
        public static List<MovieResponse> ToMovieResponse(this IEnumerable<Movie> movies)
        {
            return movies.Select(movie => movie.ToMovieResponse()).ToList();
        }

        public static MovieResponse ToMovieResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                Duration = movie.Duration,
                Genres = movie.Genres.ToGenreResponse()
            };
        }

        public static Movie FromDTO(this MovieRegisterRequest movieRegister)
        {
            return new Movie()
            {
                Title = movieRegister.Title,
                Description = movieRegister.Description,
                ReleaseYear = movieRegister.ReleaseYear,
                Duration = movieRegister.Duration
            };
        }
    }
}
