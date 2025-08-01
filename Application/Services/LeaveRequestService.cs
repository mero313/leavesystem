using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;

namespace LeaveRequestSystem.Application.Services
{
    public class LeaveRequestService
    {
        private readonly ILeaveRequestRepository leaveRequestRepository;

        public LeaveRequestService(ILeaveRequestRepository repo)
        {
            this.leaveRequestRepository = repo;
        }

        public async Task<LeaveRequestResponseDto> CreateLeaveRequestAsync(LeaveRequestRequestDto dto, int userId)
        {
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
