namespace BookeryWebApi.Dtos
{
    public class Token
    {
        public string AccessToken { get; set; }
        public RefreshTokenDto RefreshToken { get; set; }
    }
}
