using System;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.Exceptions;

namespace MovieCollection.Domain
{
    public abstract class BaseEntity
    {
        protected BaseEntity() { }

        public BaseEntity(Guid id = default)
        {
            if (id != default)
                this.Id = id;
        }

        public void SetId(Guid id)
        {
            if (id == default)
                throw new ValidationException(nameof(this.Id), string.Format(ValidationMessages.PropertyCantBeNull, nameof(this.Id)));

            this.Id = id;
        }

        public Guid Id { get; private set; } = default;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}
