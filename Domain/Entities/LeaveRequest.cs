using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public LeaveType LeaveType { get; set; }

        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public LeaveStatus Status { get; set; }

        // ✅ خزن UTC فقط
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Manager decision
        public int? ApprovedByManagerId { get; set; }
        public User? ApprovedByManager { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }
        public string? ManagerComments { get; set; }

        // (اختياري) HR decision
        public int? ApprovedByHRId { get; set; }
        public User? ApprovedByHR { get; set; }
        public DateTime? HRApprovalDate { get; set; }
        public string? HRComments { get; set; }

        
         public ICollection<LeaveRequestHistory> LeaveRequestHistory  { get; set; } = new List<LeaveRequestHistory>();
    }
}
