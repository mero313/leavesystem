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

        public HRService(ILeaveRequestRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        public async Task<LeaveRequestResponseDto> ApproveByHRAsync(int leaveId, int hrId, string? reason)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId) ?? throw new Exception("Request not found");

            if (leave.Status != LeaveStatus.ManagerApproved) throw new InvalidOperationException("Must be ManagerApproved first");

            leave.Status = LeaveStatus.HRApproved;
            leave.ApprovedByHRId = hrId;
            leave.HRApprovalDate = DateTime.UtcNow;
            leave.HRComments = reason;

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }

        public async Task<LeaveRequestResponseDto> RejectByHRAsync(int leaveId, int hrId, string reason)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId) ?? throw new Exception("Request not found");

            if (leave.Status != LeaveStatus.ManagerApproved) throw new InvalidOperationException("Must be ManagerApproved first");

            leave.Status = LeaveStatus.Rejected;
            leave.ApprovedByHRId = hrId;
            leave.HRApprovalDate = DateTime.UtcNow;
            leave.HRComments = reason;

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }
    }
}

    
