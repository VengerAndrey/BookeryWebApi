namespace BookeryWebApi.Dtos.Responses
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; }
        public RefreshTokenDto RefreshToken { get; set; }
    }
}
