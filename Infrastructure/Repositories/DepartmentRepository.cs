using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Domain.Repositories;
using LeaveRequestSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace LeaveRequestSystem.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppData _db;

        public DepartmentRepository(AppData db)
        {
            _db = db;
        }

        public async Task<Department?> GetDepartmentByIdAsync(int DepId, CancellationToken ct = default)
        {
            return await _db.Departments.FindAsync(DepId, ct);

        }

        public async Task<bool> DepartmentExistsAsync(int depId)
      => await _db.Departments.AnyAsync(d => d.Id == depId);


        public async Task AddAsync(Department department, CancellationToken ct = default)
        {
            _db.Departments.Add(department);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Department department, CancellationToken ct = default)
        {
            _db.Departments.Update(department);
            await _db.SaveChangesAsync(ct);
        }
        public async Task RemoveAsync(int id)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department is null) return; // أو ارمي استثناء إذا تحب
            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Department>> ListAsync(CancellationToken ct = default)
            => await _db.Departments
                        .AsNoTracking()
                        .OrderBy(d => d.Name)
                        .ToListAsync(ct);

        public Task<Department?> GetByManagerIdAsync(int managerUserId, CancellationToken ct = default)
            => _db.Departments.AsNoTracking()
            .FirstOrDefaultAsync(d => d.ManagerId == managerUserId, ct);
    }
}