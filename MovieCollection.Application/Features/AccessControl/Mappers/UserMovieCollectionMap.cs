using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Application.Features.AccessControl.Mappers
{
    public static class UserMovieCollectionMap
    {
        public static UserMovieCollection ToUserMovieCollection(this User user)
        {
            return new UserMovieCollection()
            {
                MovieCollection = user.MovieCollection.ToMovieResponse()
            };
        }
    }
}
