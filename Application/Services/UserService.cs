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

        public async Task Promotion2Manager(int userId, CancellationToken ct = default)
        {
            var user = await _users.GetUserByIdAsync(userId, ct) ?? throw new KeyNotFoundException("User not found");

            if (user.Role == Role.MANAGER) throw new InvalidOperationException("User is already a manager");


            user.Role = Role.MANAGER;

            await _users.UpdateAsync(user, ct);
        }

        public async Task Demote2Employee(int userId, bool removeFromDepartment, CancellationToken ct = default)
        {
            var user = await _users.GetUserByIdAsync(userId, ct) ?? throw new KeyNotFoundException("User not found");

            if (user.Role == Role.EMPLOYEE) throw new InvalidOperationException("User is already a employee");
            // 1) إذا هو مدير قسم: فك ربط القسم من هذا المدير
            var dep = await _departments.GetByManagerIdAsync(user.Id, ct);
            if (dep is not null)
            {
                dep.ManagerId = null;
                await _departments.UpdateAsync(dep, ct);
            }

            // 2) فك المرؤوسين عنه (إذا عندك هالدالة)
            var subs = await _users.ListByManagerIdAsync(user.Id, ct); // يرجع List<User>
            if (subs.Count > 0)
            {
                foreach (var s in subs)
                    s.ManagerId = null;

                await _users.UpdateRangeAsync(subs, ct);
            }

            // 3) نزّل الدور و نظّف ارتباطاته
            user.Role = Role.EMPLOYEE;
            user.ManagerId = null;

            if (removeFromDepartment)
                user.DepartmentId = null; // إذا تريد يرجع “بلا قسم”

            await _users.UpdateAsync(user, ct);
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
            await _users.UpdateAsync(user, ct);
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
