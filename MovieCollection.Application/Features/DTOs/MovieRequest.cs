using System;
using System.Collections.Generic;

namespace MovieCollection.Application.Features.DTOs
{
    public class MovieRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public TimeSpan Duration { get; set; }
        public List<GenreRequest> Genres { get; set; } = new List<GenreRequest>();
    }
}
