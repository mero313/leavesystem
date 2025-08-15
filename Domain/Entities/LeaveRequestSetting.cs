using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Domain.Entities
{
    public class LeaveSettings
    {
        public int Id { get; set; }

        // اربط بالـ Department ككيان
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public LeaveType LeaveType { get; set; }

        public int MinDaysPerRequest { get; set; }
        public int MaxDaysPerRequest { get; set; }
        public int MaxDaysPerYear { get; set; }

        public bool RequiresManagerApproval { get; set; } = true;
        public bool RequiresHRApproval { get; set; } = true;
    }
}
