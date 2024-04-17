using System;
using MovieCollection.Application.Features.AccessControl.DTOs;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Application.Features.AccessControl.Mappers
{
    public static class UserRegisterMap
    {
        public static User FromDTO(this UserRegisterRequest userRegister)
        {
            return new User
            {
                UserName = userRegister.Email,
                Email = userRegister.Email
            };
        }
    }
}
