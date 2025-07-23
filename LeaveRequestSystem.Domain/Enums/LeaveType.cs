namespace LeaveRequestSystem.Domain.Enums
{
    public enum LeaveStatus
    {
        Pending,           // في الانتظار
        Manager_approved,  // موافقة المدير
        Hr_pproved,       // موافقة الموارد البشرية
        Rejected,         // مرفوضة
        Cancelled       // ملغاة
    }
}