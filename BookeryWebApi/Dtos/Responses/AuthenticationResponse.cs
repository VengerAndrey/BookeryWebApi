namespace BookeryWebApi.Dtos.Responses
{
    public class AuthenticationResponse
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
