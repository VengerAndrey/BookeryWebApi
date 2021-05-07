namespace Domain.Models.DTOs
{
    public class UserDto : DomainObject
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
