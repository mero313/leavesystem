// Domain/Repositories/IUserRepository.cs
using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
    public interface IAuthRepository 
    {
        Task<User?> GetByUsernameAsync(string username  );
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetUsers();
        
    }
}
