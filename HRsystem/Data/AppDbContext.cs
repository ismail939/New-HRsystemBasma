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

            // ===== Payroll Module Configurations =====

            // PayrollComponent
            modelBuilder.Entity<PayrollComponent>()
                .HasIndex(pc => pc.Code)
                .IsUnique();

            modelBuilder.Entity<PayrollComponent>()
                .Property(pc => pc.Category)
                .HasConversion<int>();

            modelBuilder.Entity<PayrollComponent>()
                .Property(pc => pc.CalculationMethod)
                .HasConversion<int>();

            // EmployeePayrollComponent
            modelBuilder.Entity<EmployeePayrollComponent>()
                .HasIndex(epc => new { epc.EmployeeId, epc.PayrollComponentId, epc.StartDate })
                .IsUnique();

            modelBuilder.Entity<EmployeePayrollComponent>()
                .HasOne(epc => epc.HREmployee)
                .WithMany()
                .HasForeignKey(epc => epc.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeePayrollComponent>()
                .HasOne(epc => epc.PayrollComponent)
                .WithMany(pc => pc.EmployeeAssignments)
                .HasForeignKey(epc => epc.PayrollComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollComponentHistory
            modelBuilder.Entity<PayrollComponentHistory>()
                .HasOne(pch => pch.HREmployee)
                .WithMany()
                .HasForeignKey(pch => pch.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollComponentHistory>()
                .HasOne(pch => pch.PayrollComponent)
                .WithMany()
                .HasForeignKey(pch => pch.PayrollComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollComponentHistory>()
                .HasOne(pch => pch.ChangedByUser)
                .WithMany()
                .HasForeignKey(pch => pch.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // PayrollPolicy
            modelBuilder.Entity<PayrollPolicy>()
                .Property(pp => pp.DailySalaryCalcMethod)
                .HasConversion<int>();

            modelBuilder.Entity<PayrollPolicy>()
                .Property(pp => pp.OvertimeBase)
                .HasConversion<int>();

            modelBuilder.Entity<PayrollPolicy>()
                .Property(pp => pp.LateDeductionMethod)
                .HasConversion<int>();

            modelBuilder.Entity<PayrollPolicy>()
                .Property(pp => pp.AbsenceDeductionMethod)
                .HasConversion<int>();

            modelBuilder.Entity<PayrollPolicy>()
                .Property(pp => pp.LeaveEncashmentMethod)
                .HasConversion<int>();

            // EmployeePayrollPolicy
            modelBuilder.Entity<EmployeePayrollPolicy>()
                .HasOne(epp => epp.HREmployee)
                .WithMany()
                .HasForeignKey(epp => epp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeePayrollPolicy>()
                .HasOne(epp => epp.PayrollPolicy)
                .WithMany(pp => pp.EmployeeAssignments)
                .HasForeignKey(epp => epp.PayrollPolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // DepartmentPayrollPolicy
            modelBuilder.Entity<DepartmentPayrollPolicy>()
                .HasOne(dpp => dpp.HRDepartment)
                .WithMany()
                .HasForeignKey(dpp => dpp.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DepartmentPayrollPolicy>()
                .HasOne(dpp => dpp.PayrollPolicy)
                .WithMany(pp => pp.DepartmentAssignments)
                .HasForeignKey(dpp => dpp.PayrollPolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // OvertimePolicy
            modelBuilder.Entity<OvertimePolicy>()
                .Property(op => op.HourlyRateMethod)
                .HasConversion<int>();

            // EmployeeOvertimePolicy
            modelBuilder.Entity<EmployeeOvertimePolicy>()
                .HasOne(eop => eop.HREmployee)
                .WithMany()
                .HasForeignKey(eop => eop.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeOvertimePolicy>()
                .HasOne(eop => eop.OvertimePolicy)
                .WithMany(op => op.EmployeeAssignments)
                .HasForeignKey(eop => eop.OvertimePolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // OvertimeEntry
            modelBuilder.Entity<OvertimeEntry>()
                .HasOne(oe => oe.HREmployee)
                .WithMany()
                .HasForeignKey(oe => oe.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OvertimeEntry>()
                .HasOne(oe => oe.OvertimePolicy)
                .WithMany(op => op.OvertimeEntries)
                .HasForeignKey(oe => oe.OvertimePolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OvertimeEntry>()
                .Property(oe => oe.OvertimeType)
                .HasConversion<int>();

            modelBuilder.Entity<OvertimeEntry>()
                .HasIndex(oe => new { oe.EmployeeId, oe.Date });

            // CommissionPlan
            modelBuilder.Entity<CommissionPlan>()
                .Property(cp => cp.Type)
                .HasConversion<int>();

            // CommissionTier
            modelBuilder.Entity<CommissionTier>()
                .HasOne(ct => ct.CommissionPlan)
                .WithMany(cp => cp.Tiers)
                .HasForeignKey(ct => ct.CommissionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // EmployeeCommissionPlan
            modelBuilder.Entity<EmployeeCommissionPlan>()
                .HasOne(ecp => ecp.HREmployee)
                .WithMany()
                .HasForeignKey(ecp => ecp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeCommissionPlan>()
                .HasOne(ecp => ecp.CommissionPlan)
                .WithMany(cp => cp.EmployeeAssignments)
                .HasForeignKey(ecp => ecp.CommissionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // CommissionTransaction
            modelBuilder.Entity<CommissionTransaction>()
                .HasOne(ct => ct.HREmployee)
                .WithMany()
                .HasForeignKey(ct => ct.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommissionTransaction>()
                .HasOne(ct => ct.CommissionPlan)
                .WithMany(cp => cp.Transactions)
                .HasForeignKey(ct => ct.CommissionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommissionTransaction>()
                .HasIndex(ct => new { ct.EmployeeId, ct.TransactionDate });

            // Payroll
            modelBuilder.Entity<Payroll>()
                .Property(p => p.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Payroll>()
                .HasOne(p => p.PayrollPolicy)
                .WithMany(pp => pp.Payrolls)
                .HasForeignKey(p => p.PayrollPolicyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Payroll>()
                .HasIndex(p => new { p.Month, p.Year })
                .IsUnique();

            // PayrollItem
            modelBuilder.Entity<PayrollItem>()
                .HasOne(pi => pi.Payroll)
                .WithMany(p => p.PayrollItems)
                .HasForeignKey(pi => pi.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PayrollItem>()
                .HasOne(pi => pi.HREmployee)
                .WithMany()
                .HasForeignKey(pi => pi.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollItem>()
                .HasOne(pi => pi.PayrollComponent)
                .WithMany(pc => pc.PayrollItems)
                .HasForeignKey(pi => pi.PayrollComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollItem>()
                .Property(pi => pi.SourceType)
                .HasConversion<int>();

            modelBuilder.Entity<PayrollItem>()
                .HasIndex(pi => new { pi.PayrollId, pi.EmployeeId });

            modelBuilder.Entity<PayrollItem>()
                .HasIndex(pi => new { pi.SourceType, pi.SourceId });

            // DeductionLimitPolicy
            modelBuilder.Entity<DeductionLimitPolicy>()
                .HasOne(dlp => dlp.PayrollPolicy)
                .WithMany()
                .HasForeignKey(dlp => dlp.PayrollPolicyId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== Disciplinary Module Configurations =====

            // PenaltyRule
            modelBuilder.Entity<PenaltyRule>()
                .HasIndex(pr => pr.Code)
                .IsUnique();

            modelBuilder.Entity<PenaltyRule>()
                .Property(pr => pr.Category)
                .HasConversion<int>();

            // PenaltyLevel
            modelBuilder.Entity<PenaltyLevel>()
                .HasOne(pl => pl.PenaltyRule)
                .WithMany(pr => pr.Levels)
                .HasForeignKey(pl => pl.PenaltyRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PenaltyLevel>()
                .Property(pl => pl.ValueType)
                .HasConversion<int>();

            modelBuilder.Entity<PenaltyLevel>()
                .Property(pl => pl.DeductionUnit)
                .HasConversion<int>();

            modelBuilder.Entity<PenaltyLevel>()
                .HasIndex(pl => new { pl.PenaltyRuleId, pl.SequenceOrder })
                .IsUnique();

            // PenaltyEscalation
            modelBuilder.Entity<PenaltyEscalation>()
                .HasOne(pe => pe.PenaltyRule)
                .WithMany(pr => pr.Escalations)
                .HasForeignKey(pe => pe.PenaltyRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PenaltyEscalation>()
                .Property(pe => pe.DeductionUnit)
                .HasConversion<int>();

            modelBuilder.Entity<PenaltyEscalation>()
                .HasIndex(pe => new { pe.PenaltyRuleId, pe.OccurrenceCount })
                .IsUnique();

            // EmployeePenalty
            modelBuilder.Entity<EmployeePenalty>()
                .HasOne(ep => ep.HREmployee)
                .WithMany()
                .HasForeignKey(ep => ep.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeePenalty>()
                .HasOne(ep => ep.PenaltyRule)
                .WithMany(pr => pr.EmployeePenalties)
                .HasForeignKey(ep => ep.PenaltyRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeePenalty>()
                .HasOne(ep => ep.PenaltyLevel)
                .WithMany()
                .HasForeignKey(ep => ep.PenaltyLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeePenalty>()
                .HasOne(ep => ep.PenaltyEscalation)
                .WithMany()
                .HasForeignKey(ep => ep.PenaltyEscalationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeePenalty>()
                .HasOne(ep => ep.ApprovedByUser)
                .WithMany()
                .HasForeignKey(ep => ep.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeePenalty>()
                .Property(ep => ep.Status)
                .HasConversion<int>();

            modelBuilder.Entity<EmployeePenalty>()
                .Property(ep => ep.DeductionUnit)
                .HasConversion<int>();

            modelBuilder.Entity<EmployeePenalty>()
                .HasIndex(ep => new { ep.EmployeeId, ep.Status });

            modelBuilder.Entity<EmployeePenalty>()
                .HasIndex(ep => new { ep.EmployeeId, ep.IncidentDate });
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

        // ===== New Payroll Module Entities =====
        public DbSet<PayrollComponent> PayrollComponents { get; set; }
        public DbSet<EmployeePayrollComponent> EmployeePayrollComponents { get; set; }
        public DbSet<PayrollComponentHistory> PayrollComponentHistories { get; set; }
        public DbSet<PayrollPolicy> PayrollPolicies { get; set; }
        public DbSet<EmployeePayrollPolicy> EmployeePayrollPolicies { get; set; }
        public DbSet<DepartmentPayrollPolicy> DepartmentPayrollPolicies { get; set; }
        public DbSet<OvertimePolicy> OvertimePolicies { get; set; }
        public DbSet<EmployeeOvertimePolicy> EmployeeOvertimePolicies { get; set; }
        public DbSet<OvertimeEntry> OvertimeEntries { get; set; }
        public DbSet<CommissionPlan> CommissionPlans { get; set; }
        public DbSet<CommissionTier> CommissionTiers { get; set; }
        public DbSet<EmployeeCommissionPlan> EmployeeCommissionPlans { get; set; }
        public DbSet<CommissionTransaction> CommissionTransactions { get; set; }
        public DbSet<PayrollItem> PayrollItems { get; set; }
        public DbSet<DeductionLimitPolicy> DeductionLimitPolicies { get; set; }

        // ===== Disciplinary Module Entities =====
        public DbSet<PenaltyRule> PenaltyRules { get; set; }
        public DbSet<PenaltyLevel> PenaltyLevels { get; set; }
        public DbSet<PenaltyEscalation> PenaltyEscalations { get; set; }
        public DbSet<EmployeePenalty> EmployeePenalties { get; set; }
    }
}