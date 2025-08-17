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


            // HR decision relationship (optional)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApprovedByHR)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedByHRId)
                .OnDelete(DeleteBehavior.Restrict);


            // موجودة مسبقًا:
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApprovedByManager)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ↔ Manager
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany(m => m.Subordinates)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ↔ Department
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<LeaveRequestHistory>(b =>
    {
        b.HasKey(h => h.Id);

        b.HasOne(h => h.LeaveRequest)
       .WithMany(l => l.LeaveRequestHistory)                 // تأكّد LeaveRequest يحتوي ICollection<LeaveRequestHistory> History
       .HasForeignKey(h => h.LeaveRequestId)
       .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(h => h.ActionByUser)
       .WithMany()
       .HasForeignKey(h => h.ActionByUserId)
       .OnDelete(DeleteBehavior.Restrict);
    });


            // LeaveSettings ↔ Department
            modelBuilder.Entity<LeaveSettings>(b =>
            {
                b.HasKey(s => s.Id);

                b.HasOne(s => s.Department)
                 .WithMany(d => d.Settings)
                 .HasForeignKey(s => s.DepartmentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // كل قسم + نوع إجازة = إعداد واحد فقط
                b.HasIndex(s => new { s.DepartmentId, s.LeaveType }).IsUnique();
            });

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();



            modelBuilder.Entity<Department>()
       .HasIndex(d => d.ManagerId)
       .IsUnique(); // يسمح بعدة NULLs، لكن يمنع تكرار نفس الـ Id

        }
    }
}
