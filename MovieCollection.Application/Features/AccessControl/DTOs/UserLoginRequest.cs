using System.ComponentModel.DataAnnotations;

namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserLoginRequest
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
