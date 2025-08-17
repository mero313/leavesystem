using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetDepartmentByIdAsync(int DepId , CancellationToken ct = default);
         Task<Department?> GetByManagerIdAsync(int managerUserId, CancellationToken ct = default);
        Task<bool> DepartmentExistsAsync(int depId );
        Task<List<Department>> ListAsync(CancellationToken ct = default);   // كل الأقسام
        // كتابة
        Task AddAsync(Department department , CancellationToken ct = default);
        Task UpdateAsync(Department department , CancellationToken ct = default);
        Task RemoveAsync(int id);
    }
}