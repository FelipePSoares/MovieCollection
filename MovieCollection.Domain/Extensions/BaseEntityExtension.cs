using System.Collections.Generic;
using System.Linq;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Domain.Extensions
{
    public static class BaseEntityExtension
    {
        public static AppResponse IsValid(this IEnumerable<BaseEntity> baseEntities)
        {
            var result = baseEntities.Select(baseEntity => baseEntity.IsValid()).ToList();

            if (result.Any(r => !r.Succeeded))
                return AppResponse.Error(result.SelectMany(r => r.Messages));

            return AppResponse.Success();
        }
    }
}
