using System.Collections.Generic;
using System.Linq;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Application.Features.AccessControl.Mappers
{
    public static class UserProfileMap
    {
        public static List<UserProfileResponse> ToUserProfileResponse(this List<User> users)
        {
            return users.Select(user => user.ToUserProfileResponse()).ToList();
        }

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
