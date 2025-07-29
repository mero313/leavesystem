
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LeaveRequestSystem.Domain.Enums;




namespace LeaveRequestSystem.Application.DTOs
{

    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public Role Role { get; set; }
    }
}


