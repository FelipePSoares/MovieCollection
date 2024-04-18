using System;
using System.Collections.Generic;

namespace MovieCollection.Application.Features.DTOs
{
    public class MovieResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public TimeSpan Duration { get; set; }
        public List<GenreResponse> Genres { get; set; } = new List<GenreResponse>();
    }
}
