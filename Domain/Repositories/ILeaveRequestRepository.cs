using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Domain.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task AddAsync(LeaveRequest entity);
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<List<LeaveRequest>> GetByUserIdAsync(int userId);
        Task<List<LeaveRequest>> GetAllAsync();
        // Task<List<LeaveRequest>> GetAllCountAsync(int userId);

        Task<IEnumerable<LeaveRequest>> GetPendingByManagerIdAsync(int managerId);
        Task<IEnumerable<LeaveRequest>> GetManagerApprovedRequests(int managerId);

        Task<LeaveRequest> UpdateAsync(LeaveRequest entity); // 👈 ضرورية

        Task<bool> ExistsOverlapAsync(int userId, DateTime from, DateTime to);
        Task<List<LeaveRequest>> GetAllCountAsync(int userId);

    }
}
