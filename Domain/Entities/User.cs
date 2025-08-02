using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;




namespace LeaveRequestSystem.Domain.Entities
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Email { get; set; } 
        public string Department { get; set; } = null!;


        public Role Role { get; set; }



        ///////////////////////إضافة خصائص مهمة للإجازات
        public int? ManagerId { get; set; }  // من هو المدير المباشر
        public User? Manager { get; set; }   // Navigation property للمدير
        /// <summary>
        /// ////////
        /// </summary>
        /// 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow + TimeSpan.FromHours(3);
        public bool IsActive { get; set; } = true;

        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();    
        //public ICollection<LeaveRequest> ApprovedLeaveRequests { get; set; } = new List<LeaveRequest>(); // الإجازات التي تمت الموافقة عليها من قبل هذا المستخدم    
    }
}