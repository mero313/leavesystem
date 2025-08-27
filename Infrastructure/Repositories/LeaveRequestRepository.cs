using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Infrastructure.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppData _db;

        public LeaveRequestRepository(AppData db)
        {
            _db = db;
        }

        public async Task AddAsync(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _db.LeaveRequests
                .Include(l => l.ApprovedByManager) // حتى ManagerName يظهر بالمابر
                .Include(l => l.ApprovedByHR) // حتى HRName يظهر بالمابر
                .Include(l => l.User) // حتى UserName يظهر بالمابر
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<LeaveRequest>> GetByUserIdAsync(int userId)
        {
            return await _db.LeaveRequests
                .Include(l => l.User)
                .Where(lr => lr.UserId == userId)
                .OrderByDescending(lr => lr.Id)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAllAsync()
        {
            return await _db.LeaveRequests
                .AsNoTracking()
                .Include(l => l.User)
                .OrderByDescending(l => l.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingByManagerIdAsync(int managerId)
        {
            return await _db.LeaveRequests
                .Include(l => l.User)
                .Where(l => l.User.ManagerId == managerId && l.Status == LeaveStatus.Pending)
                .OrderByDescending(l => l.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetManagerApprovedRequests(int managerId)
        {
            return await _db.LeaveRequests
                .Include(l => l.User)
                .Where(l => l.User.ManagerId == managerId && l.Status == LeaveStatus.ManagerApproved)
                .OrderByDescending(l => l.Id)
                .ToListAsync();
        }

        public async Task<LeaveRequest> UpdateAsync(LeaveRequest entity)
        {
            // لو الكيان مِتتبَّع (tracked)، يكفي تغيّر حقوله قبل النداء وتستدعي SaveChanges
            _db.LeaveRequests.Update(entity);
            entity.UpdatedAt = DateTime.UtcNow; // لو عندك UpdatedAt
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsOverlapAsync(int userId, DateTime from, DateTime to)
        {
            return await _db.LeaveRequests
                .AnyAsync(l => l.UserId == userId &&
                               l.Status != LeaveStatus.Rejected && // عدّل حسب منطقك
                               l.FromDate < to && from < l.ToDate); // شرط التداخل
        }

        // public async Task<int> GetAllCountAsync(int userId)
        // {
        //     return await _db.LeaveRequests
        //         .Where(lr => lr.UserId == userId) // adjust field name if different
        //         .CountAsync();
        // }

        public async Task<List<LeaveRequest>> GetAllCountAsync(int userId)
        {
            return await _db.LeaveRequests
                .Include(l => l.User)
                .Where(lr => lr.UserId == userId)
                .OrderByDescending(lr => lr.Id)
                .ToListAsync();
        }
        

    }
}
