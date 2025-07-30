
using LeaveRequestSystem.Domain.Enums;


namespace LeaveRequestSystem.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.EMPLOYEE;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } =string.Empty;
        public string Department { get; set; } = string.Empty;
        
    }
}