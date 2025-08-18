namespace LeaveRequestSystem.Application.DTOs
{
    public class DepartmentWithStatsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int? ManagerId { get; set; }
        public int UsersCount { get; set; }
        public List<string>? UserNames { get; set; } // اختياري
        public string? ManagerName { get; set; }
    }
}
