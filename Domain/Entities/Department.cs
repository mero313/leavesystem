using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Api.Controllers;

namespace LeaveRequestSystem.Domain.Entities
{
    public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ManagerId { get; set; }         // نخزّن الـ UserId
    public string? ManagerName { get; set; }    // نخزّن اسم المدير

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<LeaveSettings> Settings { get; set; } = new List<LeaveSettings>();
}

}
