namespace LeaveRequestSystem.Application.DTOs
{
    public class LeaveRequestListResponse
    {

        public int Count { get; set; }
        public List<LeaveRequestResponseDto> LeaveRequests { get; set; } = new();
    }
}