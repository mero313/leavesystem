
using LeaveRequestSystem.Domain.Enums;


namespace LeaveRequestSystem.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.EMPLOYEE;
        public string CreatedAt { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

    

