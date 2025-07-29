using LeaveRequestSystem.Application.DTOs;


namespace LeaveRequestSystem.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; } = new ();
    }
}
