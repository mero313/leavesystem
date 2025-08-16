using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppData _db;
        public UserRepository(AppData db) => _db = db;

        public async Task<User?> GetByUsernameAsync(string username)
            => await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByIdAsync(int id)
            => await _db.Users.FindAsync(id);

        public async Task<List<User>> GetUsers()
            => await _db.Users.OrderByDescending(u => u.Id).ToListAsync();

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
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
    }
}
