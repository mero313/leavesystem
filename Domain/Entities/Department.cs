using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaveRequestSystem.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string? Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<LeaveSettings> Settings { get; set; } = new List<LeaveSettings>();
    }
}
