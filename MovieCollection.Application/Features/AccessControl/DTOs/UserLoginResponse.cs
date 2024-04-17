namespace MovieCollection.Application.Features.AccessControl.DTOs
{
    public class UserLoginResponse
    {
        public UserLoginResponse(string accessToken, string refreshToken)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }

        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
