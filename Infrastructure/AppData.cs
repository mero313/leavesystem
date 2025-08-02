using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Domain.Entities;

namespace LeaveRequestSystem.Infrastructure.Data
{
    public class AppData : DbContext
    {
        public AppData(DbContextOptions<AppData> options) : base(options) { }
        public AppData() { }

        public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=LeaveSystem.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تعريف العلاقة بين الإجازة والمدير اللي وافق
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApprovedByManager) // اسم الخاصية النيفيجيشن
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedByManagerId) // اسم الـ FK
                .OnDelete(DeleteBehavior.Restrict);

            // تقدر تضيف هنا علاقات إضافية حسب الحاجة
        }
    }
}
