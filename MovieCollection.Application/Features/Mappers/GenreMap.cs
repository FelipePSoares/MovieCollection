using System.Collections.Generic;
using System.Linq;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Domain;

namespace MovieCollection.Application.Features.Mappers
{
    public static class GenreMap
    {
        public static List<GenreResponse> ToGenreResponse(this IEnumerable<Genre> genres)
        {
            return genres.Select(genre => genre.ToGenreResponse()).ToList();
        }

        public static GenreResponse ToGenreResponse(this Genre genre)
        {
            return new GenreResponse
            {
                Id = genre.Id,
                Name = genre.Name,
            };
        }

        public static List<GenreUpdateRequest> ToGenreUpdate(this IEnumerable<Genre> genres)
        {
            return genres.Select(genre => genre.ToGenreUpdate()).ToList();
        }

        public static GenreUpdateRequest ToGenreUpdate(this Genre genre)
        {
            return new GenreUpdateRequest
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public static List<Genre> FromDTO(this IEnumerable<GenreUpdateRequest> genres)
        {
            return genres.Select(genre => genre.FromDTO()).ToList();
        }

        public static Genre FromDTO(this GenreUpdateRequest genre)
        {
            return new Genre
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
    }
}
