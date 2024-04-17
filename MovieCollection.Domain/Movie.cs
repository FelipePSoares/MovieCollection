using System;
using System.Collections.Generic;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Domain
{
    public class Movie : BaseEntity
    {
        public Movie() { }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public TimeSpan Duration { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<User> Users { get; set; } = new List<User>();
    }
}
