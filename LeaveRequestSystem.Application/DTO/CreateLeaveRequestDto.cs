
using System;
using LeaveRequestSystem.Models;
using System.ComponentModel.DataAnnotations;

public class CreateLeaveRequestDto
{
    [Required(ErrorMessage = "تاريخ البداية مطلوب")]
    public DateTime FromDate { get; set; }

    [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
    public DateTime ToDate { get; set; }

    public LeaveType LeaveType { get; set; }

    [StringLength(500, ErrorMessage = "سبب الإجازة يجب أن يكون أقل من 500 حرف")]
    public string Reason { get; set; } = string.Empty;
}