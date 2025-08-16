// UserDto.cs
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // ✅ بدّلنا string Department إلى حقلين:
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public int? ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public Role Role { get; set; }
        public bool IsActive { get; set; }
    }
}
