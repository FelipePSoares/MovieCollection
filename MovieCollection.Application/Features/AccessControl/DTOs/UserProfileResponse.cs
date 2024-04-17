using System;
using System.Collections.Generic;
using MovieCollection.Application.Features.DTOs;

namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public bool HasIncompletedInformation { get; set; }
        public List<MovieResponse> MovieCollection { get; set; } = new List<MovieResponse>();
    }
}
