namespace LeaveRequestSystem.Application.DTOs
{
    // للرفض/القبول (السبب اختياري عند القبول، إجباري بالرفض)
    // لتفعيل/تعطيل حساب
    public class ToggleActiveDto
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}