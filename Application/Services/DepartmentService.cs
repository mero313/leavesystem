using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Enums;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Mappers;

namespace LeaveRequestSystem.Application.Services
{
    public class DepartmentService
    {
        private readonly IDepartmentRepository _department;
        private readonly IUserRepository _users;

        public DepartmentService(IDepartmentRepository department, IUserRepository users)
        {
            _department = department;
            _users = users;
        }

        public async Task<DepartmentWithStatsDto> GetDepByIdAsync(int id, CancellationToken ct = default)
        {
            var department = await _department.GetDepartmentByIdAsync(id, ct);
            if (department is null)
                throw new KeyNotFoundException("Department not found");

            // إمّا تستخدم Mapper...
            return DepartmentMapper.ToResponseDto(department);
        }



        public async Task<List<DepartmentWithStatsDto>> GetAllDepsAsync()
        {
            var departments = await _department.ListAsync();
            if (departments == null || !departments.Any())
                throw new KeyNotFoundException("No departments found");

            return departments.Select(DepartmentMapper.ToResponseDto).ToList();
        }

        public async Task<DepartmentWithStatsDto> AddDep(DepartmentRequestDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Department name is required");

            User? manager = null;
            if (dto.ManagerUserId is int managerUserId)
            {
                manager = await _users.GetUserByIdAsync(managerUserId, ct)
                            ?? throw new KeyNotFoundException("user not found");

                if (!manager.IsActive)
                    throw new InvalidOperationException(" user is not active");

                var existingDep = await _department.GetByManagerIdAsync(manager.Id, ct);
                if (existingDep is not null)
                    throw new InvalidOperationException(
                        $"User already manages department '{existingDep.Name}' (Id={existingDep.Id}).");
            }
            var department = new Department
            {
                Name = dto.Name,
                ManagerId = manager?.Id,
                ManagerName = manager?.Name
            };


            await _department.AddAsync(department, ct);
            if (manager is not null)
            {
                if (manager.Role != Role.MANAGER)
                    manager.Role = Role.MANAGER;

                manager.DepartmentId = department.Id;
                await _users.UpdateAsync(manager, ct);
            }
            return DepartmentMapper.ToResponseDto(department);

        }

        // قسم واحد (+ اختيار أسماء الموظفين)
        public async Task<DepartmentWithStatsDto> GetDepartmentWithStatsAsync(int id, bool includeNames = false, CancellationToken ct = default)
        {
            var dep = await _department.GetDepartmentByIdAsync(id, ct)
                      ?? throw new KeyNotFoundException("Department not found");

            // عدد الموظفين لهذا القسم
            var counts = await _users.CountEmployeesPerDepartmentAsync(ct);
            counts.TryGetValue(dep.Id, out var count);

            var data = DepartmentMapper.ToResponseDto(dep);
            data.UsersCount = count;

            if (includeNames)
            {
                data.UserNames = await _users.GetEmployeeNamesByDepartmentAsync(dep.Id, ct);
            }

            return data;
        }


        // كل الأقسام مع عدد الموظفين
        public async Task<List<DepartmentWithStatsDto>> ListDepartmentsWithCountsAsync(CancellationToken ct = default)
        {
            var deps = await _department.ListAsync(ct);
            var counts = await _users.CountEmployeesPerDepartmentAsync(ct); // Dictionary

            var list = deps.Select(d => new DepartmentWithStatsDto
            {
                Id = d.Id,
                Name = d.Name,
                UsersCount = counts.TryGetValue(d.Id, out var c) ? c : 0
            })
            .OrderBy(d => d.Name)
            .ToList();

            return list;
        }
    }
}

