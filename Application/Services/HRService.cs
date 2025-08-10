using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Enums;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;

namespace LeaveRequestSystem.Application.Services
{
    public class HRService
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IUserRepository _userRepository;

        public HRService(ILeaveRequestRepository leaveRepository, IUserRepository userRepository)
        {
            _leaveRepository = leaveRepository;
            _userRepository = userRepository;
        }

        // HR Final Approval
        public async Task<LeaveRequestResponseDto> ApproveByHRAsync(int leaveId, int hrUserId)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new Exception("Leave request not found!");

            // Verify the user is HR
            var hrUser = await _userRepository.GetByIdAsync(hrUserId);
            if (hrUser == null || hrUser.Role != Role.HR)
                throw new UnauthorizedAccessException("Only HR can perform this action!");

            // Check if manager already approved
            if (leave.Status != LeaveStatus.Manager_approved)
                throw new InvalidOperationException("Leave must be approved by manager first!");

            if (leave.Status == LeaveStatus.Hr_approved )
                throw new InvalidOperationException("Leave request is already approved by HR!");

            if (leave.Status == LeaveStatus.Rejected)
                throw new InvalidOperationException("Leave request is rejected by HR!");

            leave.Status = LeaveStatus.Hr_approved;
            leave.UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(3);

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }

        // HR Rejection
        public async Task<LeaveRequestResponseDto> RejectByHRAsync(int leaveId, int hrUserId, string reason)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new Exception("Leave request not found!");

            var hrUser = await _userRepository.GetByIdAsync(hrUserId);
            if (hrUser == null || hrUser.Role != Role.HR)
                throw new UnauthorizedAccessException("Only HR can perform this action!");

            if (leave.Status == LeaveStatus.Hr_approved)
                throw new InvalidOperationException("Leave request is already approved by HR!");

            leave.Status = LeaveStatus.Rejected;
            leave.UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(3);

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }

        // Get all pending HR approvals
        public async Task<IEnumerable<LeaveRequestResponseDto>> GetPendingHRApprovalsAsync()
        {
            var leaves = await _leaveRepository.GetAllAsync();
            var pendingHR = leaves.Where(l => l.Status == LeaveStatus.Manager_approved);

            return pendingHR.Select(LeaveRequestMapper.ToResponseDto).ToList();
        }

        // Get all leave requests for HR dashboard
        public async Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeavesForHRAsync()
        {
            var leaves = await _leaveRepository.GetAllAsync();
            return leaves.Select(LeaveRequestMapper.ToResponseDto).ToList();
        }

        // Get leave statistics for HR dashboard
        public async Task<LeaveStatisticsDto> GetLeaveStatisticsAsync()
        {
            var leaves = await _leaveRepository.GetAllAsync();

            return new LeaveStatisticsDto
            {
                TotalRequests = leaves.Count,
                PendingRequests = leaves.Count(l => l.Status == LeaveStatus.Pending),
                ManagerApproved = leaves.Count(l => l.Status == LeaveStatus.Manager_approved),
                HRApproved = leaves.Count(l => l.Status == LeaveStatus.Hr_approved),
                Rejected = leaves.Count(l => l.Status == LeaveStatus.Rejected),
                Cancelled = leaves.Count(l => l.Status == LeaveStatus.Cancelled)
            };
        }
    }
}