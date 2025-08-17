using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;
using LeaveRequestSystem.Application.Services;
using LeaveRequestSystem.Domain.Enums;
using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Application.Services
{
    public class LeaveRequestService
    {
        private readonly ILeaveRequestRepository leaveRequestRepository;
        private readonly IUserRepository _userRepository;
        public LeaveRequestService(ILeaveRequestRepository repo, IUserRepository userRepository)
        {
            this._userRepository = userRepository;

            this.leaveRequestRepository = repo;

        }

        public async Task<LeaveRequestResponseDto> CreateLeaveRequestAsync(LeaveRequestRequestDto dto, int userId ,  CancellationToken ct = default)
        {
            // 1) التحقق من المستخدم
            var user = await _userRepository.GetUserByIdAsync(userId ,ct)
                       ?? throw new UnauthorizedAccessException("User not authenticated");

            // 2) التحقق من التواريخ
            if (dto.FromDate.Date >= dto.ToDate.Date)
                throw new ArgumentException("End date must be after start date");

            // (اختياري) منع تقديم إجازة بتاريخ ماضي
            if (dto.FromDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Start date cannot be in the past");

            // 3) لازم يكون مرتبط بمدير
            if (user.ManagerId == null)
                throw new InvalidOperationException("الموظف غير مرتبط بأي مدير، يرجى ربط الحساب بمدير.");

            // 4) التحقق من وجود المدير وصحته
            var manager = await _userRepository.GetUserByIdAsync(user.ManagerId.Value , ct);
            if (manager == null || manager.Role != Role.MANAGER || !manager.IsActive)
                throw new InvalidOperationException("مدير غير صالح أو غير موجود.");

            // 5) التحقق من القسم (إذا شرط أساسي عندك)
            if (user.DepartmentId == null)
                throw new InvalidOperationException("الموظف غير مرتبط بأي قسم.");

            // 6) (اختياري) منع تداخل الطلبات لنفس المستخدم
            // افترض عندك دالة في الريبو:
            // if (await leaveRequestRepository.ExistsOverlapAsync(userId, dto.FromDate, dto.ToDate))
            //     throw new InvalidOperationException("لديك طلب إجازة يتداخل مع نفس الفترة.");

            var entity = LeaveRequestMapper.CreateLeaveRequest(dto, userId);
            await leaveRequestRepository.AddAsync(entity);
            return LeaveRequestMapper.ToResponseDto(entity);
        }

        public async Task<List<LeaveRequestResponseDto>> GetRequestsForUserAsync(int userId)
        {
            var requests = await leaveRequestRepository.GetByUserIdAsync(userId);
            return requests.Select(LeaveRequestMapper.ToResponseDto).ToList();
        }

        public async Task<LeaveRequestResponseDto?> GetRequestByIdAsync(int id)
        {
            var request = await leaveRequestRepository.GetByIdAsync(id);
            return request != null ? LeaveRequestMapper.ToResponseDto(request) : null;
        }

        public async Task<List<LeaveRequestResponseDto>> GetAllRequestsAsync()
        {
            var requests = await leaveRequestRepository.GetAllAsync();
            return requests.Select(LeaveRequestMapper.ToResponseDto).ToList();
        }
    }
}
