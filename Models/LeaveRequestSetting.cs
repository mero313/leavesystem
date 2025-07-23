using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveRequestSystem.Models;

 public class LeaveSettings
    {
        public int Id { get; set; }
        public string Department { get; set; } = null!;
        public LeaveType LeaveType { get; set; }
        public int MaxDaysPerYear { get; set; }
        public int MinDaysPerRequest { get; set; }
        public int MaxDaysPerRequest { get; set; }
        public bool RequiresManagerApproval { get; set; } = true;
        public bool RequiresHRApproval { get; set; } = true;
    }
