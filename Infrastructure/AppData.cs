using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Domain.Entities;
using LeaveRequestSystem.Entities;

namespace LeaveRequestSystem.Data
{
    public class AppData : DbContext
    {
        public AppData(DbContextOptions<AppData> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        // إذا عدك جداول بعد، ضيفها هنا بنفس الطريقة

        // (اختياري) Configure Models/Relations in OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // مثلاً:
            // modelBuilder.Entity<LeaveRequest>().HasOne<User>(...);
        }
    }
}
