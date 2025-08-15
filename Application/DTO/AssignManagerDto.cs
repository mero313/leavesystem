namespace LeaveRequestSystem.Application.DTOs 
{
 
   // لتعيين/سحب مدير
    public class AssignManagerDto
    {
        public int UserId { get; set; }
        public int? DepartmentId { get; set; }
        public bool PromoteToManager { get; set; } = true;
    }
}