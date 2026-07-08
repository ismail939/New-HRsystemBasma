using HRsystem.Models;
using HRsystem.Models.Enums;
using Microsoft.EntityFrameworkCore;
namespace HRsystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HREmployeeBasma>()
            .HasIndex(x => new { x.EmployeeId, x.DayDate })
            .IsUnique();

            modelBuilder.Entity<HREmployee>()
               .HasIndex(e => e.NationalId)
               .IsUnique()
               .HasFilter("[NationalId] IS NOT NULL");

            modelBuilder.Entity<HREmployee>()
               .HasOne(e => e.HRDepartment)
               .WithMany(d => d.Employees)
               .HasForeignKey(e => e.HRDepartmentId);

            // Request relationships
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.RespondedByUser)
                .WithMany()
                .HasForeignKey(r => r.RespondedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification relationships
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedRequest)
                .WithMany(r => r.Notifications)
                .HasForeignKey(n => n.RelatedRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            // Store enums as ints
            modelBuilder.Entity<Request>()
                .Property(r => r.RequestType)
                .HasConversion<int>();

            modelBuilder.Entity<Request>()
                .Property(r => r.LeaveType)
                .HasConversion<int>();

            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Notification>()
                .Property(n => n.NotificationType)
                .HasConversion<int>();

            // Indexes for performance
            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.CreatedAt);

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.Status);

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.CreatedAt);
        }

        public DbSet<HREmployee> HREmployees { get; set; }
        public DbSet<HREmployeeFile> HREmployeeFiles { get; set; }
        public DbSet<HREmployeeBasma> HREmployeeBasmas { get; set; }
        public DbSet<HREmployeeOffDay> HREmployeeOffDays { get; set; }
        public DbSet<HREmployeePenalty> HREmployeePenalties { get; set; }
        public DbSet<HREmployeeRate> HREmployeeRates { get; set; }
        public DbSet<HRApplier> HRAppliers { get; set; }
        public DbSet<HRApplierFile> HRApplierFiles { get; set; }
        public DbSet<HREmployeeShift> HREmployeeShift { get; set; }
        public DbSet<HROffDayBalance> HROffDayBalances { get; set; }
        public DbSet<CheckInOut> CheckInOuts { get; set; }
        public DbSet<DailyBasmaFlag> DailyBasmaFlags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HRDepartment> HRDepartments { get; set; }
        public DbSet<HRLog> HRLogs { get; set; }
        public DbSet<HRShiftOption> HRShiftOptions { get; set; }
        public DbSet<ShiftOverride> ShiftOverrides {get; set;}

        // New entities
        public DbSet<Request> Requests { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Payroll entities
        public DbSet<SalaryComponent> SalaryComponents { get; set; }
        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        public DbSet<SalaryHistory> SalaryHistories { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PayrollDetail> PayrollDetails { get; set; }
        public DbSet<PayrollEarning> PayrollEarnings { get; set; }
        public DbSet<PayrollDeduction> PayrollDeductions { get; set; }
    }
}
