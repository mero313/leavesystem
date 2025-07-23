using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Application.DTO;
using LeaveRequestSystem.Domain.Enums

namespace LeaveRequestSystem.Application.Mappers
{
    public static class LeaveRequestMapper
    {
        public static LeaveRequest ToEntity(CreateLeaveRequestDto dto)
        {
            return new LeaveRequest
            {
                UserId = dto.UserId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Reason = dto.Reason,
                LeaveType = (LeaveType)dto.LeaveType, // Cast if using enum
                Status = LeaveStatus.Pending, // Default to pending, for example
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
