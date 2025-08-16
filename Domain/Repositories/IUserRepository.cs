// Domain/Repositories/IUserRepository.cs
using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
  public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(int id );
    // 
    Task<List<User>> GetUsers();
    Task UpdateAsync(User user); // ✅ ضرورية
    // (اختياري) تسهيلات:
     Task<List<User>> GetByManagerIdAsync(int managerId);
     Task<List<User>> GetByDepartmentIdAsync(int departmentid);
}

}
