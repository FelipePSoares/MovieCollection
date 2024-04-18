using System.Collections.Generic;
using System;

namespace MovieCollection.Application.Features.DTOs
{
    public class MovieRegisterRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
