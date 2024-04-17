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
    }
}
