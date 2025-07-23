using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace LeaveRequestSystem.Models
{
    public enum Role
    {
        HR,
        MANAGER,
        EMPLOYEE
    }

    public enum LeaveType
    {
        ANNUAL,    // إجازة سنوية
        SICK,      // إجازة مرضية  
        TEMPORARY  // إجازة مؤقتة
    }

    public enum LeaveStatus
    {
        PENDING,           // في الانتظار
        MANAGER_APPROVED,  // موافقة المدير
        HR_APPROVED,       // موافقة الموارد البشرية
        REJECTED,          // مرفوضة
        CANCELLED          // ملغاة
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = null!;

        public Role Role { get; set; }

        // إضافة خصائص مهمة للإجازات
        public int? ManagerId { get; set; }  // من هو المدير المباشر
        public User? Manager { get; set; }   // Navigation property للمدير

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // العلاقات
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<User> Subordinates { get; set; } = new List<User>(); // الموظفين تحت إدارته
    }
}