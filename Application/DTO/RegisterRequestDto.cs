// RegisterRequestDto.cs
using LeaveRequestSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LeaveRequestSystem.Application.DTOs
{
    public class RegisterRequestDto
    {
       [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
       
        
    }
}
