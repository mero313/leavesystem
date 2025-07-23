using Microsoft.EntityFrameworkCore;
using LeaveRequestSystem.Models;

namespace LeaveRequestSystem.Data
{
    public class AppData : DbContext
    {
        public AppData(DbContextOptions<AppData> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveRequestHistory> LeaveRequestHistories { get; set; }
        public DbSet<LeaveSettings> LeaveSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إعداد User Entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                // الفهارس
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Department);

                // العلاقة التفاعلية مع النفس (Manager - Subordinates)
                entity.HasOne(u => u.Manager)
                    .WithMany(u => u.Subordinates)
                    .HasForeignKey(u => u.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // تحويل Enum إلى String
                entity.Property(u => u.Role)
                    .HasConversion<string>();
            });

            // إعداد LeaveRequest Entity
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.Id);

                // الفهارس
                entity.HasIndex(lr => lr.UserId);
                entity.HasIndex(lr => lr.Status);
                entity.HasIndex(lr => lr.FromDate);

                // العلاقة مع User
                entity.HasOne(lr => lr.User)
                    .WithMany(u => u.LeaveRequests)
                    .HasForeignKey(lr => lr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // العلاقة مع المدير الذي وافق
                entity.HasOne(lr => lr.ApprovedByManager)
                    .WithMany()
                    .HasForeignKey(lr => lr.ApprovedByManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // العلاقة مع HR الذي وافق
                entity.HasOne(lr => lr.ApprovedByHR)
                    .WithMany()
                    .HasForeignKey(lr => lr.ApprovedByHRId)
                    .OnDelete(DeleteBehavior.Restrict);

                // تحويل Enums إلى String
                entity.Property(lr => lr.LeaveType)
                    .HasConversion<string>();

                entity.Property(lr => lr.Status)
                    .HasConversion<string>();
            });

            // إعداد LeaveRequestHistory Entity
            modelBuilder.Entity<LeaveRequestHistory>(entity =>
            {
                entity.HasKey(lrh => lrh.Id);

                // العلاقة مع LeaveRequest
                entity.HasOne(lrh => lrh.LeaveRequest)
                    .WithMany()
                    .HasForeignKey(lrh => lrh.LeaveRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                // العلاقة مع User الذي قام بالإجراء
                entity.HasOne(lrh => lrh.ActionByUser)
                    .WithMany()
                    .HasForeignKey(lrh => lrh.ActionByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // تحويل Enums إلى String
                entity.Property(lrh => lrh.FromStatus)
                    .HasConversion<string>();

                entity.Property(lrh => lrh.ToStatus)
                    .HasConversion<string>();
            });

            // إعداد LeaveSettings Entity
            modelBuilder.Entity<LeaveSettings>(entity =>
            {
                entity.HasKey(ls => ls.Id);

                // مؤشر مركب - كل قسم ونوع إجازة له إعداد واحد فقط
                entity.HasIndex(ls => new { ls.Department, ls.LeaveType })
                    .IsUnique();

                // تحويل Enum إلى String
                entity.Property(ls => ls.LeaveType)
                    .HasConversion<string>();
            });

            // إضافة بيانات أولية
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // إضافة المدير العام
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "hash",
                    Name = "Super Admin",
                    Email = "admin@example.com",
                    Department = "HR",      // <<< أضف هذا السطر
                    Role = Role.HR,
                    CreatedAt = new DateTime(2024, 7, 1, 10, 0, 0)
                });
        }
    }
}