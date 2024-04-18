using System;

namespace MovieCollection.Application.Features.DTOs
{
    public class GenreUpdateRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
