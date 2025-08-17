using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppData _db;
        public UserRepository(AppData db) => _db = db;

        public async Task<User?> GetByUsernameAsync(string username)
            => await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default)
            => await _db.Users.FindAsync(id);

        public async Task<List<User>> GetUsers()
            => await _db.Users.OrderByDescending(u => u.Id).ToListAsync();

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<List<User>> GetByManagerIdAsync(int managerId)
        {
            return await _db.Users.Where(u => u.ManagerId == managerId).ToListAsync();
        }

        public async Task<List<User>> GetByDepartmentIdAsync(int departmentId)
        => await _db.Users
                .Where(u => u.DepartmentId == departmentId)
                .ToListAsync();


        // عدد الموظفين (Role = EMPLOYEE) لكل قسم: Dictionary<DepartmentId, Count>
        public async Task<Dictionary<int, int>> CountEmployeesPerDepartmentAsync(CancellationToken ct = default)
        {
            var data = await _db.Users
                .Where(u => u.Role == Role.EMPLOYEE && u.DepartmentId != null)
                .GroupBy(u => u.DepartmentId!.Value)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            return data.ToDictionary(x => x.DepartmentId, x => x.Count);
        }


        // أسماء الموظفين داخل قسم معيّن
        public Task<List<string>> GetEmployeeNamesByDepartmentAsync(int departmentId, CancellationToken ct = default)
            => _db.Users
                  .Where(u => u.Role == Role.EMPLOYEE && u.DepartmentId == departmentId)
                  .Select(u => u.Name)
                  .AsNoTracking()
                  .ToListAsync(ct);
    }
}
