namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserRefreshTokenResponse
    {
        public UserRefreshTokenResponse(string accessToken, string refreshToken)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }

        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
