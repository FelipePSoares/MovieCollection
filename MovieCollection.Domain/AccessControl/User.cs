using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MovieCollection.Domain.AccessControl
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public bool HasIncompletedInformation { get; set; } = true;
        public List<Movie> MovieCollection { get; set; } = new List<Movie>();
    }
}
