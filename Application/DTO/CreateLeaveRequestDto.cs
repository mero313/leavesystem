
using System;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Enums;


namespace LeaveRequestSystem.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a leave request.
    /// </summary>

    public class CreateLeaveRequestDto

    {
        // public int UserId { get; set; }
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateTime ToDate { get; set; }

        public LeaveType LeaveType { get; set; } = LeaveType.Annual; // Default value

        [StringLength(500, ErrorMessage = "سبب الإجازة يجب أن يكون أقل من 500 حرف")]
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}