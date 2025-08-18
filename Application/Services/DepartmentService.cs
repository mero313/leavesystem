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

        public async Task<DepartmentWithStatsDto> CreateAsync(DepartmentRequestDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Department name is required");

            var byName = await _department.GetDepByNameAsync(dto.Name, ct);
            if (byName is not null)
                throw new InvalidOperationException($"Department '{dto.Name}' already exists.");


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

            if (manager?.Role != Role.MANAGER)
                throw new InvalidOperationException("Provided user is not a manager.");

            var department = new Department
            {
                Name = dto.Name,
                ManagerId = (manager is not null && manager.Role == Role.MANAGER)
                    ? manager.Id
                    : null
            };


            await _department.AddAsync(department, ct);

            if (manager is not null && manager.Role == Role.MANAGER)
            {
                // من الممارسات الجيدة: المدير ينتمي لنفس القسم الذي يديره
                manager.DepartmentId = department.Id;
                await _users.UpdateAsync(manager, ct);
            }
            return DepartmentMapper.ToResponseDto(department);

        }
        public async Task<DepartmentWithStatsDto> AssignManagerAsync(int departmentId, int managerUserId, CancellationToken ct = default)
        {
            // 1) تأكد من القسم
            var department = await _department.GetDepartmentByIdAsync(departmentId, ct)
                              ?? throw new KeyNotFoundException("Department not found");

            // 2) تأكد من المستخدم
            var manager = await _users.GetUserByIdAsync(managerUserId, ct)
                           ?? throw new KeyNotFoundException("User not found");

            if (!manager.IsActive)
                throw new InvalidOperationException("User is not active");

            // 3) تأكد أنه مدير
            if (manager.Role != Role.MANAGER)
                throw new InvalidOperationException("Provided user is not a manager");

            // 4) تأكد أن المستخدم ما يدير قسم ثاني
            var existingDep = await _department.GetByManagerIdAsync(manager.Id, ct);
            if (existingDep is not null && existingDep.Id != departmentId)
                throw new InvalidOperationException(
                    $"User already manages department '{existingDep.Name}' (Id={existingDep.Id}).");

            // 5) حدث القسم وحدد المدير
            department.ManagerId = manager.Id;
            await _department.UpdateAsync(department, ct);

            // 6) اربط المدير بالقسم
            manager.DepartmentId = department.Id;
            await _users.UpdateAsync(manager, ct);

            // 7) رجّع DTO للـ frontend
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

            if (dep.ManagerId is int mid)
            {
                var manager = await _users.GetUserByIdAsync(mid, ct);
                data.ManagerName = manager?.Name;
            }

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
        public async Task AddEmployeeAsync(int departmentId, int userId, bool moveIfInAnotherDept = false, CancellationToken ct = default)
        {
            // ) تأكّد من القِسم
            var dept = await _department.GetDepartmentByIdAsync(departmentId, ct)
                      ?? throw new KeyNotFoundException("Department not found");

            // ) تأكّد من المستخدم
            var user = await _users.GetUserByIdAsync(userId, ct)
                      ?? throw new KeyNotFoundException("User not found");

            if (!user.IsActive)
                throw new InvalidOperationException("User is not active");

            if (user.Role != Role.EMPLOYEE)
                throw new InvalidOperationException("Only EMPLOYEE users can be assigned via this operation");


            // ) إذا أصلاً بنفس القسم → لا شي
            if (user.DepartmentId == departmentId)
                return;

            if (user.DepartmentId is int currentDeptId && currentDeptId != departmentId)
            {
                if (!moveIfInAnotherDept)
                    throw new InvalidOperationException("User belongs to another department. Set moveIfInAnotherDept=true to move.");
            }

            // ) عين القسم
            user.DepartmentId = departmentId;
            user.ManagerId = dept.ManagerId;
            await _users.UpdateAsync(user, ct);

        }
        ///إضافة مجموعة موظفين (دفعة واحدة)
        public async Task AddEmployeesAsync(int departmentId, AddEmployeesToDepartmentDto dto, CancellationToken ct = default)
        {
            var dept = await _department.GetDepartmentByIdAsync(departmentId, ct)
                      ?? throw new KeyNotFoundException("Department not found");

            if (dto.UserIds == null || dto.UserIds.Count == 0)
                throw new ArgumentException("No users specified");

            // نستخدم معاملة إذا كانت البنية عندك تدعم UnitOfWork (نفس DbContext)
            // pseudo:
            // using var tx = await _uow.BeginTransactionAsync(ct);
            // try { ... await _uow.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
            // catch { await tx.RollbackAsync(ct); throw; }

            // نفّذ واحدًا واحدًا (تقدر تحسّنها بجلب دفعة واحدة من الداتابيز)
            foreach (var userId in dto.UserIds.Distinct())
            {
                var user = await _users.GetUserByIdAsync(userId, ct);
                if (user is null)
                    throw new KeyNotFoundException($"User {userId} not found");

                if (!user.IsActive)
                    throw new InvalidOperationException($"User {userId} is not active");

                if (user.Role == Role.MANAGER && user.DepartmentId != null && user.DepartmentId != departmentId)
                {
                    if (!dto.MoveIfAlreadyInAnotherDepartment)
                        throw new InvalidOperationException($"User {userId} is a manager of another department");
                    // سياسة النقل إن رغبت...
                }

                if (user.DepartmentId == departmentId)
                    continue; // موجود أصلاً

                user.DepartmentId = departmentId;
                await _users.UpdateAsync(user, ct);
            }

            // لو عندك UoW, خزّن مرّة وحدة هنا
        }

        public async Task RemoveEmployeeAsync(int departmentId, int userId, CancellationToken ct = default)
        {
            var dept = await _department.GetDepartmentByIdAsync(departmentId, ct)
                      ?? throw new KeyNotFoundException("Department not found");

            var user = await _users.GetUserByIdAsync(userId, ct)
                      ?? throw new KeyNotFoundException("User not found");

            if (user.DepartmentId != departmentId)
                return; // أو throw لو تحب تشدد

            // لو هو مدير القسم؟ قرر السياسة:
            if (user.Role == Role.MANAGER && dept.ManagerId == user.Id)
                throw new InvalidOperationException("Cannot remove the department manager");

            user.DepartmentId = null;
            await _users.UpdateAsync(user, ct);
        }



    }
}

