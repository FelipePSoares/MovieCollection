using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Application.Features.AccessControl.Mappers
{
    public static class UserProfileMap
    {
        public static UserProfileResponse ToUserProfileResponse(this User user)
        {
            return new UserProfileResponse()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MovieCollection = user.MovieCollection.ToMovieResponse(),
                Enabled = user.Enabled,
                HasIncompletedInformation = user.HasIncompletedInformation
            };
        }
    }
}
