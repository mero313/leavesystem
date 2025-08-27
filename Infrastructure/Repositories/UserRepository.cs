using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppData _db;
        public UserRepository(AppData db) => _db = db;

        public async Task<User?> GetByUsernameAsync(string username)
            => await _db.Users.FirstOrDefaultAsync(u => u.Username == username);



        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

            

        public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default)
            => await _db.Users.FindAsync(id);

        public async Task<List<User>> GetUsers(CancellationToken ct = default)
            => await _db.Users
        .Include(u => u.Department)   // ✅ حتى يجي DepartmentName
        .Include(u => u.Manager)      // ✅ حتى يجي ManagerName (إذا تحتاجه)
        .AsNoTracking()               // قراءة فقط
        .OrderByDescending(u => u.Id)
        .ToListAsync(ct);


        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<User>> GetByManagerIdAsync(int managerId)
        {
            return await _db.Users.Where(u => u.ManagerId == managerId).ToListAsync();
        }

        public async Task<List<User>> GetByDepartmentIdAsync(int departmentId)
        => await _db.Users
                .Where(u => u.DepartmentId == departmentId)
                .ToListAsync();


        // عدد الموظفين (Role = EMPLOYEE) لكل قسم: Dictionary<DepartmentId, Count>
        public async Task<Dictionary<int, int>> CountEmployeesPerDepartmentAsync(CancellationToken ct = default)
        {
            var data = await _db.Users
                .Where(u => u.Role == Role.EMPLOYEE && u.DepartmentId != null)
                .GroupBy(u => u.DepartmentId!.Value)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            return data.ToDictionary(x => x.DepartmentId, x => x.Count);
        }


        // أسماء الموظفين داخل قسم معيّن
        public Task<List<string>> GetEmployeeNamesByDepartmentAsync(int departmentId, CancellationToken ct = default)
            => _db.Users
                  .Where(u => u.Role == Role.EMPLOYEE && u.DepartmentId == departmentId)
                  .Select(u => u.Name)
                  .AsNoTracking()
                  .ToListAsync(ct);



        public async Task<List<User>> ListByManagerIdAsync(int managerId, CancellationToken ct = default)
        {
            return await _db.Users
                .Where(u => u.ManagerId == managerId)
                .ToListAsync(ct);
        }


        public async Task UpdateRangeAsync(IEnumerable<User> users, CancellationToken ct = default)
        {
            _db.Users.UpdateRange(users);
            await _db.SaveChangesAsync(ct);
        }


        public async Task<PagedResult<UserDto>> GetUsersPagedDtoAsync(UserQuery q, CancellationToken ct = default)
        {
            IQueryable<User> baseQuery = _db.Users
                                        .AsNoTracking();


            // Filters
            if (q.DepartmentId is int depId) baseQuery = baseQuery.Where(u => u.DepartmentId == depId);
            if (q.ManagerId is int manId) baseQuery = baseQuery.Where(u => u.ManagerId == manId);
            if (q.IsActive is bool act) baseQuery = baseQuery.Where(u => u.IsActive == act);
            if (q.Role is Role role) baseQuery = baseQuery.Where(u => u.Role == role);


            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var t = q.Search.ToLower();
                baseQuery = baseQuery.Where(u =>
                    u.Username.ToLower().Contains(t) ||
                    u.Name.ToLower().Contains(t) ||
                    (u.Email != null && u.Email.ToLower().Contains(t)) ||
                    (u.Department != null && u.Department.Name.ToLower().Contains(t)));
            }

            // Total before paging
            var total = await baseQuery.CountAsync(ct);

            // Sorting
            baseQuery = (q.SortBy?.ToLowerInvariant()) switch
            {
                "name" => (q.Desc ? baseQuery.OrderByDescending(u => u.Name) : baseQuery.OrderBy(u => u.Name)),
                "username" => (q.Desc ? baseQuery.OrderByDescending(u => u.Username) : baseQuery.OrderBy(u => u.Username)),
                "createdat" => (q.Desc ? baseQuery.OrderByDescending(u => u.CreatedAt) : baseQuery.OrderBy(u => u.CreatedAt)),
                // department / manager sorting رح نسويه بعد الـ Select (أقل مثالية SQLيًا، لكن بسيط)
                "department" => baseQuery.OrderBy(u => u.Id), // placeholder؛ راح نرتب بعد الـ Select
                "manager" => baseQuery.OrderBy(u => u.Id), // placeholder
                _ => (q.Desc ? baseQuery.OrderByDescending(u => u.Id) : baseQuery.OrderBy(u => u.Id)),
            };

            // Paging
            var pageSize = Math.Clamp(q.PageSize, 1, 100);
            var page = Math.Max(q.Page, 1);



            var projected = baseQuery
                  .Select(u => new UserDto
                  {
                      Id = u.Id,
                      Username = u.Username,
                      Name = u.Name,
                      Email = u.Email ?? "",
                      DepartmentId = u.DepartmentId,
                      DepartmentName = u.Department != null ? u.Department.Name : "", // EF يحولها LEFT JOIN
                      ManagerId = u.ManagerId,
                      ManagerName = u.Manager != null ? u.Manager.Name : "",
                      Role = u.Role,
                      IsActive = u.IsActive,
                      CreatedAt = u.CreatedAt,
                  });



            var items = await projected
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(ct);

            return new PagedResult<UserDto>(items, total, page, pageSize);
        }

    }
}
