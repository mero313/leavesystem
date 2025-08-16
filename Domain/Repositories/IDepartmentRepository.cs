using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetDepartmentByIdAsync(int DepId);
        Task<bool> DepartmentExistsAsync(int depId );
        Task<List<Department>> ListAsync();   // كل الأقسام
        // كتابة
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task RemoveAsync(int id);
    }
}