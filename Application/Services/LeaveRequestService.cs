using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;

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

        public async Task<LeaveRequestResponseDto> CreateLeaveRequestAsync(LeaveRequestRequestDto dto, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not authenticated");
            }

            if (user.ManagerId == null)
            {
                throw new Exception("الموظف غير مرتبط بأي مدير، يرجى ربط الحساب بمدير.");
            }
            var entity = LeaveRequestMapper.CreateLeaveRequest(dto, userId );
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
