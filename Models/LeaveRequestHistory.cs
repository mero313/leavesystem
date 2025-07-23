using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Models;



public class LeaveRequestHistory
{
    public int Id { get; set; }
    public int LeaveRequestId { get; set; }
    public LeaveRequest LeaveRequest { get; set; } = null!;

    public LeaveStatus FromStatus { get; set; }
    public LeaveStatus ToStatus { get; set; }

    public int ActionByUserId { get; set; }
    public User ActionByUser { get; set; } = null!;

    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    public string? Comments { get; set; }
}