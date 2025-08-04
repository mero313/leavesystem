using System;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Domain.Enums;



namespace LeaveRequestSystem.Application.DTOs
{
    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; }
        public string CreatedAt { get; set; } = string.Empty; // Assuming this is a formatted date string
    }
}
