using System;
using System.Collections.Generic;
using System.Linq;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Domain
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public override AppResponse IsValid()
        {
            var erroMessages = new List<AppMessage>();

            if (string.IsNullOrEmpty(this.Name))
                erroMessages.Add(new AppMessage(nameof(this.Name), String.Format(ValidationMessages.PropertyCantBeNullOrEmpty, nameof(this.Name))));

            if (erroMessages.Any())
                return AppResponse.Error(erroMessages);

            return AppResponse.Success();
        }
    }
}
