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
        public DbSet<LeaveRequestHistory> LeaveRequestHistory { get; set; }
        public DbSet<LeaveSettings> LeaveSettings { get; set; }   // انتبه: اسم الكلاس عندك "LeaveSettings" أو "LeaveRequestSetting"؟ خليه نفس اسم الكلاس بالضبط
        public DbSet<Department> Departments { get; set; }


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

            // ===================== Users & Departments =====================
            // User ↔ Department (Many-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Department.ManagerId → User (مدير القسم اختياري)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager) // لو عندك Navigation d.Manager: استبدلها بـ .HasOne(d => d.Manager)
                .WithOne(u => u.ManagedDepartment)
                .HasForeignKey<Department>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            // User self reference (Manager ↔ Subordinates)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany(m => m.Subordinates)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== LeaveRequest =====================
            // (1) صاحب الطلب (Required)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.User)                // Navigation: User
                .WithMany(u => u.LeaveRequests)     // Navigation على User
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // (2) المعتمد من المدير (Optional)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.ApprovedByManager)
                .WithMany()
                .HasForeignKey(l => l.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // (3) المعتمد من HR (Optional)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.ApprovedByHR)
                .WithMany()
                .HasForeignKey(l => l.ApprovedByHRId)
                .OnDelete(DeleteBehavior.Restrict);

            // قصّ الميلي ثانية لحقول الوقت داخل LeaveRequest
            var secondPrecisionConverter = new ValueConverter<DateTime, DateTime>(
                v => v.AddTicks(-(v.Ticks % TimeSpan.TicksPerSecond)),
                v => v.AddTicks(-(v.Ticks % TimeSpan.TicksPerSecond))
            );
            modelBuilder.Entity<LeaveRequest>().Property(x => x.FromDate).HasConversion(secondPrecisionConverter);
            modelBuilder.Entity<LeaveRequest>().Property(x => x.ToDate).HasConversion(secondPrecisionConverter);
            modelBuilder.Entity<LeaveRequest>().Property(x => x.CreatedAt).HasConversion(secondPrecisionConverter);
            modelBuilder.Entity<LeaveRequest>().Property(x => x.UpdatedAt).HasConversion(secondPrecisionConverter);

            // ===================== LeaveRequestHistory =====================
            modelBuilder.Entity<LeaveRequestHistory>(b =>
            {
                b.HasKey(h => h.Id);

                b.HasOne(h => h.LeaveRequest)
                 .WithMany(l => l.LeaveRequestHistory)   // تأكّد الاسم يطابق Navigation بالكيان
                 .HasForeignKey(h => h.LeaveRequestId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(h => h.ActionByUser)
                 .WithMany()
                 .HasForeignKey(h => h.ActionByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== LeaveSettings =====================
            modelBuilder.Entity<LeaveSettings>(b =>
            {
                b.HasKey(s => s.Id);

                b.HasOne(s => s.Department)
                 .WithMany(d => d.Settings)
                 .HasForeignKey(s => s.DepartmentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // قسم + نوع إجازة = إعداد واحد
                b.HasIndex(s => new { s.DepartmentId, s.LeaveType }).IsUnique();
            });

            // ===================== قيود وفهارس =====================
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(); // يسمح بعدة NULLs في SQLite
            modelBuilder.Entity<User>().Property(u => u.Username).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.PasswordHash).IsRequired();

            // لمنع تعيين نفس المدير لأكثر من قسم (اختياري)
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.ManagerId)
                .IsUnique();
        }

    }
}
