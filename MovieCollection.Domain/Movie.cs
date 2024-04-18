using System;
using System.Collections.Generic;
using System.Linq;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Domain.Extensions;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.DTOs;

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

        public override AppResponse IsValid()
        {
            var erroMessages = new List<AppMessage>();

            if (string.IsNullOrEmpty(this.Title))
                erroMessages.Add(new AppMessage(nameof(this.Title), String.Format(ValidationMessages.PropertyCantBeNullOrEmpty, nameof(this.Title))));

            if (erroMessages.Any())
                return AppResponse.Error(erroMessages);

            return AppResponse.Success();
        }
    }
}
