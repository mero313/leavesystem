 namespace LeaveRequestSystem.Application.DTOs
{
    public class HRStatisticsDto
    {

        public int users_count { get; set; }
        public int leave_Count { get; set; }
        public List<LeaveStatisticsDto> LeaveStatistics { get; set; } = new();
       
    }
}