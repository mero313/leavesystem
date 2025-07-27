
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;


namespace LeaveRequestSystem.DTOs
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.EMPLOYEE;
        public string Name { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; } =string.Empty;
        public string Department { get; set; } = string.Empty;
        
    }
}