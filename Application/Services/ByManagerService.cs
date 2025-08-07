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

        // موافقة المدير المباشر
        public async Task<LeaveRequestResponseDto> ApproveByManagerAsync(int leaveId, int managerId)
        {

            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new Exception("طلب الإجازة غير موجود!");

            var employee = await _userRepository.GetByIdAsync(leave.UserId);
            if (employee == null || employee.ManagerId != managerId)
                throw new UnauthorizedAccessException("غير مصرح لك بالموافقة على هذه الإجازة!");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("لا يمكن الموافقة على طلب ليس قيد الانتظار.");



            leave.Status = LeaveStatus.Manager_approved;
            // ممكن تخزن تاريخ الموافقة من المدير هنا
            leave.ManagerApprovalDate = DateTime.UtcNow + TimeSpan.FromHours(3); // Adjusting for timezone if necessary
            leave.ApprovedByManagerId = managerId; // حفظ معرف المدير الذي وافق
            leave.ManagerComments = "تمت الموافقة من قبل المدير"; // يمكنك تعديل التعليق


            await _leaveRepository.UpdateAsync(leave);
            return LeaveRequestMapper.ToResponseDto(leave);
        }

        // رفض من المدير المباشر
        public async Task<LeaveRequestResponseDto> RejectByManagerAsync(int leaveId, int managerId)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
                throw new Exception("طلب الإجازة غير موجود!");

            var employee = await _userRepository.GetByIdAsync(leave.UserId);
            if (employee == null || employee.ManagerId != managerId)
                throw new UnauthorizedAccessException("غير مصرح لك برفض هذه الإجازة!");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("لا يمكن رفض طلب ليس قيد الانتظار.");

            leave.Status = LeaveStatus.Rejected;

            await _leaveRepository.UpdateAsync(leave);

            return LeaveRequestMapper.ToResponseDto(leave);
        }


        // جلب طلبات الإجازة للمدير
         public async Task<IEnumerable<LeaveRequestResponseDto>> GetRequestsForManagerAsync(int managerId)
        {
            var leaves = await _leaveRepository.GetByManagerIdAsync(managerId);
            // لو تريد فقط الطلبات في حالة Pending:
             leaves = leaves.Where(l => l.Status == LeaveStatus.Pending);

            return leaves
                .Select(LeaveRequestMapper.ToResponseDto)
                .ToList();
        }
    }
}
