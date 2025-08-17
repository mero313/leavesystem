using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Enums;
using LeaveRequestSystem.Application.DTOs;

namespace LeaveRequestSystem.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _users;
        private readonly IDepartmentRepository _departments;

        public UserService(IUserRepository users, IDepartmentRepository departments)
        {
            this._users = users;
            this._departments = departments;
        }

        public async Task AssignManagerAsync(int userId, int? departmentId, bool promoteToManager , CancellationToken ct = default)
        {
            var user = await _users.GetUserByIdAsync(userId , ct) ?? throw new KeyNotFoundException("User not found");

            if (departmentId is int depId)
            {
                if (!await _departments.DepartmentExistsAsync(depId))
                    throw new KeyNotFoundException("Department not found");


                user.DepartmentId = depId; // تعيين الـ FK مباشرة
            }

            if (promoteToManager) user.Role = Role.MANAGER;
            else if (user.Role == Role.MANAGER) user.Role = Role.EMPLOYEE;

            await _users.UpdateAsync(user , ct);
        }

        public async Task ToggleActiveAsync(int userId, bool isActive, CancellationToken ct = default)
        {
            var user = await _users.GetUserByIdAsync(userId, ct) ?? throw new Exception("User not found");

            // 2. شيّك إذا حالته هي نفسها اللي نريد نغيرها
            if (user.IsActive == isActive)
            {
                throw new Exception(isActive
                    ? "User is already active"
                    : "User is already inactive");
            }

            // 3. حدّث الحالة
            user.IsActive = isActive;
            await _users.UpdateAsync(user,ct);
        }

        public async Task<List<UserDto>> GetallUsersAsync()
        {
            var users = await _users.GetUsers(); // يرجّع Entities من الريبو
            if (users == null)
                throw new Exception("Users not found");

            // نحول الـ Entity إلى DTO
                var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Name = u.Name,
                Email = u.Email ?? "",
                DepartmentId = u.DepartmentId,
                Role = u.Role,
                IsActive = u.IsActive,
            }).ToList();

            return dtos; // هنا لازم ترجع قيمة

        }

    }
}
