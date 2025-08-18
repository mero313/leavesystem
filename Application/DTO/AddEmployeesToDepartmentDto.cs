namespace LeaveRequestSystem.Application.DTOs
{
    public class AddEmployeesToDepartmentDto
    {
        public List<int> UserIds { get; set; } = new();
        public bool MoveIfAlreadyInAnotherDepartment { get; set; } = false; // إن أردت السماح بالنقل
    }
}

