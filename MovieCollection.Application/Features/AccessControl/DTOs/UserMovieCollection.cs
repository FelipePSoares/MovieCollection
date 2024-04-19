using System.Collections.Generic;
using MovieCollection.Application.Features.DTOs;

namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserMovieCollection
    {
        public List<MovieResponse> MovieCollection { get; set; } = new List<MovieResponse>();
    }
}
