namespace LeaveRequestSystem.DTOs;

using LeaveRequestSystem.Models;


public class UserResponse
{
    public int    Id       { get; set; }
    public string Username { get; set; } = null!;
    public Role Role { get; set; }
}
