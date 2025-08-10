using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            return await _db.LeaveRequests.FindAsync(id);
        }

        public async Task<List<LeaveRequest>> GetByUserIdAsync(int userId)
        {
            return await _db.LeaveRequests
                .Where(lr => lr.UserId == userId).OrderByDescending(lr => lr.Id)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAllAsync()
        {
            return await _db.LeaveRequests.ToListAsync();
        }

        public async Task UpdateAsync(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingByManagerIdAsync(int managerId)
        {
            return await _db.LeaveRequests
             .Include(l => l.User)                         // <-- مهم حتى يمتلئ UserName بالمابر
             .Where(l => l.User.ManagerId == managerId
                         && l.Status == LeaveStatus.Pending).OrderByDescending(l => l.Id)
             .ToListAsync();

        }

        public async Task<IEnumerable<LeaveRequest>> GetManagerApprovedRequests(int managerId)
        {
            return await _db.LeaveRequests
                .Include(l => l.User)
                .Where(l => l.User.ManagerId == managerId
                            && l.Status == LeaveStatus.Manager_approved).OrderByDescending(l => l.Id)
                .ToListAsync();
        }
    }
}
