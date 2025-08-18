using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Entities; // ✅ هذا فقط

namespace LeaveRequestSystem.Domain.Entities
{
    public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ManagerId { get; set; }         // نخزّن الـ UserId
    public User? Manager { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<LeaveSettings> Settings { get; set; } = new List<LeaveSettings>();
}

}
