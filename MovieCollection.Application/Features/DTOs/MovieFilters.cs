using System;
using System.Collections.Generic;

namespace MovieCollection.Application.Features.DTOs
{
    public class MovieFilters
    {
        public string? Title { get; set; } = string.Empty;
        public int? ReleaseYearStart { get; set; }
        public int? ReleaseYearEnd { get; set; }
        public List<Guid> Genres { get; set; } = new List<Guid>();
    }
}
