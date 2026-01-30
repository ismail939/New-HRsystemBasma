using HRsystem.Models;
using Microsoft.EntityFrameworkCore;
namespace HRsystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
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
        
    }
}