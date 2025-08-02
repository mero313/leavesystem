using LeaveRequestSystem.Infrastructure.Data;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;



namespace LeaveRequestSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppData _db;

        public UserRepository(AppData db)
        {
            _db = db;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<List<User>> GetUsers()
        {
            return await _db.Users.ToListAsync();
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.FindAsync(id);
        }



        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        
       
    }

}
