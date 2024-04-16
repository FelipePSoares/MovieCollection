namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserRefreshTokenResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
