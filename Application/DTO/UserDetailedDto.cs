
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Application.DTOs
{
    public class UserDetailedDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int TeamMembersCount { get; set; }


    }
}


