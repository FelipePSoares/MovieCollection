namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserRefreshTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
