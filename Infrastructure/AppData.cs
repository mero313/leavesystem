using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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


            // Converter يقصّ الميلي ثانية (يخلي الدقة لثواني فقط)
            var secondPrecisionConverter = new ValueConverter<DateTime, DateTime>(
                v => v.AddTicks(-(v.Ticks % TimeSpan.TicksPerSecond)), // عند الحفظ
                v => v.AddTicks(-(v.Ticks % TimeSpan.TicksPerSecond))  // عند القراءة
            );

            // طبّقه على الخصائص اللي تريدها (مثلاً FromDate و ToDate)
            modelBuilder.Entity<LeaveRequest>()
                .Property(x => x.ToDate)
                .HasConversion(secondPrecisionConverter);

            modelBuilder.Entity<LeaveRequest>()
                .Property(x => x.FromDate)
                .HasConversion(secondPrecisionConverter);

            modelBuilder.Entity<LeaveRequest>()
           .Property(x => x.CreatedAt)
           .HasConversion(secondPrecisionConverter);

           modelBuilder.Entity<LeaveRequest>()
           .Property(x => x.UpdatedAt)
           .HasConversion(secondPrecisionConverter);
                

            

        }
    }
}
