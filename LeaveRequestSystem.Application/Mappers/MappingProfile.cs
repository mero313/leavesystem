using LeaveRequestSystem.Domain.Entities;
using System;
using LeaveRequestSystem.Application.DTO;
using LeaveRequestSystem.Domain.Enums;



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
                LeaveType = dto.LeaveType,
                // Assuming Status is set to Pending by default, you can adjust this as needed
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
