namespace LeaveRequestSystem.Application.DTOs
{


    // Application/DTOs/DepartmentRequestDto.cs
    public class DepartmentRequestDto
    {
        public string Name { get; set; } = string.Empty;

        // optional: if provided, we'll promote this user as the manager
        public int? ManagerUserId { get; set; } 
         

        // if you still want to accept/display the name you can keep it,
        // but it will be overridden by the selected user’s Name.

    }

}