using HRsystem.Models;
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

            // Self reference
            

            modelBuilder.Entity<HREmployee>()
               .HasOne(e => e.HRDepartment)
               .WithMany(d => d.Employees)
               .HasForeignKey(e => e.HRDepartmentId);
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
    }
}