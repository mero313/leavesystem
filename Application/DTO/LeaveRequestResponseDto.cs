using System;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Application.DTOs
{
    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string managerName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; }
        // خليه string للعرض بفورمات، مثل ما يسوي المابر
        public string CreatedAt { get; set; } = string.Empty;
    }
}
