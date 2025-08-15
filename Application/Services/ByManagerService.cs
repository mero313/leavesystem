using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Enums;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;

namespace LeaveRequestSystem.Application.Services
{
    public class ByManagerService
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IUserRepository _userRepository;

        public ByManagerService(ILeaveRequestRepository leaveRepository, IUserRepository userRepository)
        {
            _leaveRepository = leaveRepository;
            _userRepository = userRepository;
        }

        public async Task<LeaveRequestResponseDto> ApproveByManagerAsync(int leaveId, int managerId, string? reason)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId) ?? throw new Exception("Request not found");
            var employee = await _userRepository.GetByIdAsync(leave.UserId) ?? throw new Exception("Employee not found");

            if (employee.ManagerId != managerId) throw new UnauthorizedAccessException("Not your subordinate");
            if (leave.Status != LeaveStatus.Pending) throw new InvalidOperationException("Invalid status");

            leave.Status = LeaveStatus.ManagerApproved;
            leave.ApprovedByManagerId = managerId;
            leave.ManagerApprovalDate = DateTime.UtcNow;
            leave.ManagerComments = reason;

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }

        public async Task<LeaveRequestResponseDto> RejectByManagerAsync(int leaveId, int managerId, string reason)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId) ?? throw new Exception("Request not found");
            var employee = await _userRepository.GetByIdAsync(leave.UserId) ?? throw new Exception("Employee not found");

            if (employee.ManagerId != managerId) throw new UnauthorizedAccessException("Not your subordinate");
            if (leave.Status != LeaveStatus.Pending) throw new InvalidOperationException("Invalid status");

            leave.Status = LeaveStatus.Rejected;
            leave.ApprovedByManagerId = managerId;
            leave.ManagerApprovalDate = DateTime.UtcNow;
            leave.ManagerComments = reason;

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }
    }
}
