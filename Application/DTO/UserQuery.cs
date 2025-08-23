using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Application.DTOs
{
    public class UserQuery
    {
        public int Page { get; set; } = 1;             // الصفحة
        public int PageSize { get; set; } = 20;        // حد أقصى 100
        public string? Search { get; set; }            // اسم/يوزر/إيميل
        public int? DepartmentId { get; set; }         // فلترة قسم
        public int? ManagerId { get; set; }            // فلترة مدير
        public bool? IsActive { get; set; }            // فلترة حالة
        public string? SortBy { get; set; } = "id";    // id,name,username,createdAt,department
        public bool Desc { get; set; } = true;         // تنازلي؟
        public Role? Role { get; set; }                // فلترة الدور (اختياري)
    }
}
