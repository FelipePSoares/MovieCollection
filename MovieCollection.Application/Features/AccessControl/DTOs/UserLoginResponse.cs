namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserLoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
