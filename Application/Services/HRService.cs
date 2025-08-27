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

            switch (leave.Status)
            {
                case LeaveStatus.ManagerApproved:
                    // الحالة الوحيدة المسموحة بالانتقال إلى HRApproved
                    break;

                case LeaveStatus.HRApproved:
                    throw new InvalidOperationException("Request is already HRApproved.");

                case LeaveStatus.Rejected:
                    throw new InvalidOperationException("Request is already Rejected by manager : " + leave.ApprovedByManager?.Username);

                default:
                    throw new InvalidOperationException("Request must be ManagerApproved before HR Approval.");
            }


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

            switch (leave.Status)
            {
                case LeaveStatus.ManagerApproved:
                    // الحالة الوحيدة المسموحة بالانتقال إلى HRApproved
                    break;

                case LeaveStatus.HRApproved:
                    throw new InvalidOperationException("Request is already HRApproved.");

                case LeaveStatus.Rejected:
                    throw new InvalidOperationException("Request is already Rejected by manager : " + leave.ApprovedByManager?.Username);

                default:
                    throw new InvalidOperationException("Request must be ManagerApproved before HR Reject.");
            }
            leave.Status = LeaveStatus.Rejected;
            leave.ApprovedByHRId = hrId;
            leave.HRApprovalDate = DateTime.UtcNow;
            leave.HRComments = reason;

            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }

        public async Task<LeaveRequestListResponse> HrPendingRequest()
        {
            var leaveRequests = await _leaveRepository.GetAllAsync();

            var data = leaveRequests
                .Where(lr => lr.Status == LeaveStatus.ManagerApproved)
                .OrderByDescending(lr => lr.Id)
                .Select(LeaveRequestMapper.ToResponseDto)
                .ToList();

            return new LeaveRequestListResponse
            {
                Count = data.Count,
                LeaveRequests = data
            };
        }

    }
}


