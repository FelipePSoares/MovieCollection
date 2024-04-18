using System;
using System.Collections.Generic;

namespace MovieCollection.Application.Features.DTOs
{
    public class MovieFilters
    {
        public string? Title { get; set; } = string.Empty;
        public DateTime? ReleaseDateStart { get; set; }
        public DateTime? ReleaseDateEnd { get; set; }
        public List<Guid> Genres { get; set; } = new List<Guid>();
    }
}
