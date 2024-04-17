namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserRefreshTokenRequest
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
