// Domain/Repositories/IUserRepository.cs
using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
  public interface IUserRepository
  {
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(User user);
    Task<User?> GetUserByIdAsync(int id, CancellationToken ct);
    // 
    Task<List<User>> GetUsers();
    Task UpdateAsync(User user, CancellationToken ct); // ✅ ضرورية
                                                       // (اختياري) تسهيلات:
    Task<List<User>> GetByManagerIdAsync(int managerId);
    Task<List<User>> GetByDepartmentIdAsync(int departmentid);

    Task<Dictionary<int, int>> CountEmployeesPerDepartmentAsync(CancellationToken ct = default);
    Task<List<string>> GetEmployeeNamesByDepartmentAsync(int departmentId, CancellationToken ct = default);
      Task<List<User>> ListByManagerIdAsync(int managerId, CancellationToken ct = default);
        Task UpdateRangeAsync(IEnumerable<User> users, CancellationToken ct = default);
  }

}
