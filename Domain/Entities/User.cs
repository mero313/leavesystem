using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;




namespace LeaveRequestSystem.Entities
{

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
        //    
    }
}