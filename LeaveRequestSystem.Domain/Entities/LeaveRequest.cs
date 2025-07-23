using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;
namespace LeaveRequestSystem.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public LeaveStatus Status { get; set; } = LeaveStatus.PENDING;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // معلومات الموافقة
        public int? ApprovedByManagerId { get; set; }
        public User? ApprovedByManager { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }
        public string? ManagerComments { get; set; }

        public int? ApprovedByHRId { get; set; }
        public User? ApprovedByHR { get; set; }
        public DateTime? HRApprovalDate { get; set; }
        public string? HRComments { get; set; }

        // Foreign Key → User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // حساب عدد الأيام
        public int TotalDays => (ToDate - FromDate).Days + 1;

        // التحقق من صحة التواريخ
        public bool IsValidDateRange => FromDate <= ToDate && FromDate >= DateTime.Today;
    }
}
