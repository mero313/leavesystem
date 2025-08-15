using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository users)
        {
            _users = users;
        }

        public async Task AssignManagerAsync(int userId, int? departmentId, bool promoteToManager)
        {
            var user = await _users.GetByIdAsync(userId) ?? throw new Exception("User not found");

            if (departmentId.HasValue) user.DepartmentId = departmentId;

            if (promoteToManager) user.Role = Role.MANAGER;
            else if (user.Role == Role.MANAGER) user.Role = Role.EMPLOYEE;

            await _users.UpdateAsync(user);
        }

        public async Task ToggleActiveAsync(int userId, bool isActive)
        {
            var user = await _users.GetByIdAsync(userId) ?? throw new Exception("User not found");
            user.IsActive = isActive;
            await _users.UpdateAsync(user);
        }
    }
}
