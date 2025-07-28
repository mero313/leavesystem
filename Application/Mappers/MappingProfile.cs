using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Enums;
using System;



namespace LeaveRequestSystem.Application.Mappers
{
    public static class LeaveRequestMapper
    {
        public static LeaveRequest CreateLeaveRequest(CreateLeaveRequestDto dto)
        {
            return new LeaveRequest
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Reason = dto.Reason,
                LeaveType = dto.LeaveType,
                // Assuming Status is set to Pending by default, you can adjust this as needed
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static LeaveRequestResponseDto ToResponseDto(LeaveRequest  entity)
        {
            return new LeaveRequestResponseDto
            {
                Id = entity.Id,   
                FromDate = entity.FromDate,
                ToDate = entity.ToDate,
                LeaveType = entity.LeaveType, // Assuming LeaveType is an enum
                Reason = entity.Reason,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") // Format as needed
            };
        }
    }
}
