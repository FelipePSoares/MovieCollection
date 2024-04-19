using System;

namespace MovieCollection.Application.Features.DTOs
{
    public class GenreRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
