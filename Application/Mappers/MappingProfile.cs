using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Enums;
using System;

namespace LeaveRequestSystem.Application.Mappers
{
    public static class LeaveRequestMapper
    {
        // من RequestDto -> Entity (للإنشاء)
        public static LeaveRequest CreateLeaveRequest(LeaveRequestRequestDto dto, int userId)
        {
            return new LeaveRequest
            {
                UserId = userId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Reason = dto.Reason,
                LeaveType = dto.LeaveType,
                Status = LeaveStatus.Pending,
                // خزّن دائمًا بتوقيت UTC. لا تزود +3 ساعات هنا.
                CreatedAt = DateTime.UtcNow,
                // إذا عندك UpdatedAt بالـ Entity خليه عند التعديل فقط
                // UpdatedAt = DateTime.UtcNow
            };
        }

        // من Entity -> ResponseDto (للعرض)
        public static LeaveRequestResponseDto ToResponseDto(LeaveRequest entity)
        {
            return new LeaveRequestResponseDto
            
            {
                Id = entity.Id,
                UserId = entity.UserId,
                managerName = entity.ApprovedByManager?.Name ?? string.Empty, // إذا كنت تعمل Include(ApprovedByManager) في الـ repo، إبقِ السطر، وإلا اشيله:
                // إذا كنت تعمل Include(User) في الـ repo، إبقِ السطر، وإلا اشيله:
                UserName = entity.User?.Username ?? string.Empty,
                FromDate = entity.FromDate,
                ToDate = entity.ToDate,
                LeaveType = entity.LeaveType,
                Reason = entity.Reason,
                Status = entity.Status,
                // الفورمات يكون في الرد فقط—not in DB
                CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }

    // مابر موحّد للّوجن/الريجيستر
    public static class AuthMapper
    {
        public static LoginResponseDto ToLoginResponseDto(User user, string token)
        {
            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name ?? string.Empty,
                    Role = user.Role,
                    IsActive = user.IsActive
                }
            };
        }

        public static User ToUserEntity(RegisterRequestDto dto)
        {
            return new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Role.EMPLOYEE,
                Name = dto.Name,
                Email = dto.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }


    }
    public static class DepartmentMapper
    {
        public static Department ToDepartmentEntity(DepartmentRequestDto dto)
        {
            return new Department
            {

                Name = dto.Name,
                ManagerId = dto.ManagerUserId


            };
        }

        // public static DepartmentWithStatsDto ToResponseDto(Department department, int usersCount, List<string>? userNames = null)
        // {
        //     return new DepartmentWithStatsDto
        //     {
        //         Id = department.Id,
        //         Name = department.Name,
        //         ManagerName = department.ManagerName,
        //         UsersCount = usersCount,
        //         UserNames = userNames

        //     };
        // }

        public static DepartmentWithStatsDto ToResponseDto(Department department)
        {
            return new DepartmentWithStatsDto
            {
                Id = department.Id,
                Name = department.Name,
                ManagerId = department.ManagerId


            };
        }



    }

    public static class UserMapper
    {
        public static UserDto UserResponseDto(User user)
        {
             return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email ?? "",
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name ?? "", // اذا محمّل الـ navigation
                ManagerId = user.ManagerId,
                ManagerName = user.Manager?.Name ?? "",
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
        public static List<UserDto> UserResponseDto(IEnumerable<User> users)
        => users.Select(UserResponseDto).ToList();
    }
}



