namespace LeaveRequestSystem.Application.DTOs
{
    // للرفض/القبول (السبب اختياري عند القبول، إجباري بالرفض)
    public class DecisionDto
    {
        public string? Reason { get; set; }
    }
}