namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserLoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
