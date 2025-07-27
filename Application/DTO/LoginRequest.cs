
using System.ComponentModel.DataAnnotations;



namespace LeaveRequestSystem.Application.DTOs

{
  public class LoginRequest
  {
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = null!;
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

  }
}

