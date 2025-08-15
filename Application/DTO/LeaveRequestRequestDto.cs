using System;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Application.DTOs
{
    public class LeaveRequestRequestDto
    {
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateTime ToDate { get; set; }

        public LeaveType LeaveType { get; set; } = LeaveType.Annual;

        [StringLength(500, ErrorMessage = "سبب الإجازة يجب أن يكون أقل من 500 حرف")]
        public string Reason { get; set; } = string.Empty;
    }
}
