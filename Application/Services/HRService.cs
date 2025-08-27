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

        public HRService(ILeaveRequestRepository leaveRepository ,IUserRepository userRepository) 
        {
            _leaveRepository = leaveRepository ;
            _userRepository = userRepository;
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

        public async Task<LeaveRequestListResponse> GetAllLeaveRequestsAsync()
        {
            var leaveRequests = await _leaveRepository.GetAllAsync();

            var data = leaveRequests
                .OrderByDescending(lr => lr.Id)
                .Select(LeaveRequestMapper.ToResponseDto)
                .ToList();

            return new LeaveRequestListResponse
            {
                Count = data.Count,
                LeaveRequests = data
            };
        }

        public async Task<HRStatisticsDto> GetHRStatisticsAsync()
        {
            var user = await _userRepository.GetUsers();
            if (user == null || user.Count == 0) throw new KeyNotFoundException("No users found");  
            var leaveRequests = await _leaveRepository.GetAllAsync();

            var stats = new HRStatisticsDto
            {   users_count = user.Count(),
                leave_Count = leaveRequests.Count,
                LeaveStatistics = new List<LeaveStatisticsDto>
                {
                    new LeaveStatisticsDto
                    {

                        PendingRequests = leaveRequests.Count(lr => lr.Status == LeaveStatus.Pending),
                          ManagerApproved = leaveRequests.Count(lr => lr.Status == LeaveStatus.ManagerApproved),
                           HRApproved = leaveRequests.Count(lr => lr.Status == LeaveStatus.HRApproved),
                             Rejected = leaveRequests.Count(lr => lr.Status == LeaveStatus.Rejected)
                    } }
            };
           
            return stats;
        }

       

    }
}


