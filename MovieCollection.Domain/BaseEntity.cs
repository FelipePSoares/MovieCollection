using System;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = default;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public abstract AppResponse IsValid();
    }
}
