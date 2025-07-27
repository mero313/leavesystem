using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task AddAsync(LeaveRequest entity);
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<List<LeaveRequest>> GetByUserIdAsync(int userId);
        Task<List<LeaveRequest>> GetAllAsync();


    }
}
