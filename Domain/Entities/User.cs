using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;


namespace LeaveRequestSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        // ✅ هوية القسم الرقمية (اختياريّة)
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Role Role { get; set; } = Role.EMPLOYEE;

        // مدير مباشر (Self-reference)
        public int? ManagerId { get; set; }
        public User? Manager { get; set; }
        public ICollection<User> Subordinates { get; set; } = new List<User>();

        public bool IsActive { get; set; } = true;

        // ✅ خزّن UTC فقط
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         

        // علاقات الإجازات
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<LeaveRequestHistory> ActionHistory { get; set; } = new List<LeaveRequestHistory>();
        public Department? ManagedDepartment { get; set; }

        
    }
}
