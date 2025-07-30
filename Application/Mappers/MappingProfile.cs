using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Enums;
using System;



namespace LeaveRequestSystem.Application.Mappers
{
    public static class LeaveRequestMapper
    {
        public static LeaveRequest CreateLeaveRequest(LeaveRequestRequestDto dto)
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
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(3) // Adjusting for timezone if necessary
            };
        }

        public static LeaveRequestResponseDto ToResponseDto(LeaveRequest entity)
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

    public static class LoginMapper
    {
        public static LoginResponseDto ToLoginResponseDto(User user, string token)
        {
            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1), // Assuming token expires in 1 hour
                User = new UserDto
                {
                    Id = user.Id,

                    Username = user.Username,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty, // Handle null email
                    Department = user.Department,
                    Role = user.Role, // Assuming Role is an enum, convert to string
                    IsActive = user.IsActive



                }
                
            };
        }
    }

           
    public static class RegisterMapper
    {
        public static User ToUserEntity(RegisterRequestDto dto)
        {
            return new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Hash the password
                Role = dto.Role,
                Name = dto.Name,
                Email = dto.Email,
                Department = dto.Department,
                IsActive = true, // Assuming new users are active by default
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(3) // Adjusting for timezone if necessary
            };
        }
    }


}
