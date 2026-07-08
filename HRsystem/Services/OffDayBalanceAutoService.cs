using HRsystem.Data;
using HRsystem.Models;
using HRsystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Services
{
    /// <summary>
    /// خدمة حساب رصيد الإجازات تلقائياً حسب نظام العمل السعودي
    /// Saudi Labor Law auto-calculation service for leave balances
    /// </summary>
    public interface IOffDayBalanceAutoService
    {
        /// <summary>
        /// حساب الرصيد التلقائي لموظف معين
        /// </summary>
        Task<HROffDayBalance> CalculateAndSaveBalanceAsync(int employeeId);
        
        /// <summary>
        /// حساب الرصيد لجميع الموظفين النشطين
        /// </summary>
        Task CalculateAllBalancesAsync();
        
        /// <summary>
        /// الحصول على الرصيد (موجود أو إنشاء حساب تلقائي)
        /// </summary>
        Task<HROffDayBalance> GetOrCalculateBalanceAsync(int employeeId);
        
        /// <summary>
        /// خصم الرصيد بعد الموافقة على طلب إجازة (يرفض لو الرصيد غير كافٍ)
        /// </summary>
        Task<bool> DeductBalanceAsync(int employeeId, LeaveType leaveType, int days);
        
        /// <summary>
        /// خصم إجباري للرصيد (لـ HR) - يسمح بالرصيد السالب
        /// </summary>
        Task<bool> ForceDeductBalanceAsync(int employeeId, LeaveType leaveType, int days);
        
        /// <summary>
        /// إضافة رصيد (عند إزالة إجازة يدوياً)
        /// </summary>
        Task<bool> AddToBalanceAsync(int employeeId, LeaveType leaveType, int days);
    }

    public class OffDayBalanceAutoService : IOffDayBalanceAutoService
    {
        private readonly AppDbContext _context;

        public OffDayBalanceAutoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HROffDayBalance> CalculateAndSaveBalanceAsync(int employeeId)
        {
            var employee = await _context.HREmployees
                .FirstOrDefaultAsync(e => e.Id == employeeId);
                
            if (employee == null)
                throw new ArgumentException($"Employee with ID {employeeId} not found");

            var balance = await _context.HROffDayBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId);

            if (balance == null)
            {
                balance = new HROffDayBalance
                {
                    EmployeeId = employeeId,
                    Notes = "",
                    LastUpdated = DateTime.UtcNow,
                    IsAutoCalculated = true
                };
                _context.HROffDayBalances.Add(balance);
            }

            // Calculate years of service
            var yearsOfService = (int)((DateTime.Today - employee.HireDate).TotalDays / 365.25);
            
            // 1. Annual leave: 21 days for <5 years, 30 days for >=5 years
            balance.Annual = yearsOfService >= 5 ? 30 : 21;
            
            // 2. Sick leave: 120 days per year (renewed annually)
            // Check if this is a new year since last update
            if (balance.LastUpdated == null || 
                balance.LastUpdated.Value.Year < DateTime.Today.Year ||
                !balance.IsAutoCalculated)
            {
                balance.Sick = 120;
            }
            
            // 3. Other balances are set to defaults (HR can adjust manually)
            balance.Casual = balance.Casual; // keep existing casual balance
            balance.Hajj = balance.Hajj; // keep existing
            balance.Maternity = balance.Maternity;
            balance.Unpaid = balance.Unpaid;
            balance.Compensatory = balance.Compensatory;
            balance.OfficialHoliday = balance.OfficialHoliday;
            balance.Exam = balance.Exam;
            
            balance.LastUpdated = DateTime.UtcNow;
            balance.IsAutoCalculated = true;
            
            await _context.SaveChangesAsync();
            return balance;
        }

        public async Task CalculateAllBalancesAsync()
        {
            var employees = await _context.HREmployees
                .Where(e => e.EndDate == null) // active employees only
                .ToListAsync();

            foreach (var emp in employees)
            {
                await CalculateAndSaveBalanceAsync(emp.Id);
            }
        }

        public async Task<HROffDayBalance> GetOrCalculateBalanceAsync(int employeeId)
        {
            var balance = await _context.HROffDayBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId);

            if (balance == null)
            {
                return await CalculateAndSaveBalanceAsync(employeeId);
            }

            // If auto-calculated and more than a day old, recalculate
            if (balance.IsAutoCalculated && 
                (DateTime.UtcNow - (balance.LastUpdated ?? DateTime.MinValue)).TotalDays > 1)
            {
                return await CalculateAndSaveBalanceAsync(employeeId);
            }

            return balance;
        }

        public async Task<bool> DeductBalanceAsync(int employeeId, LeaveType leaveType, int days)
        {
            var balance = await _context.HROffDayBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId);

            if (balance == null)
                return false;

            switch (leaveType)
            {
                case LeaveType.Annual:
                    if (balance.Annual < days) return false;
                    balance.Annual -= days;
                    break;
                case LeaveType.Casual:
                    if (balance.Casual < days) return false;
                    balance.Casual -= days;
                    break;
                case LeaveType.Sick:
                    if (balance.Sick < days) return false;
                    balance.Sick -= days;
                    break;
                case LeaveType.Hajj:
                    if (balance.Hajj < days) return false;
                    balance.Hajj -= days;
                    break;
                case LeaveType.Maternity:
                    if (balance.Maternity < days) return false;
                    balance.Maternity -= days;
                    break;
                case LeaveType.Unpaid:
                    if (balance.Unpaid < days) return false;
                    balance.Unpaid -= days;
                    break;
                case LeaveType.Compensatory:
                    if (balance.Compensatory < days) return false;
                    balance.Compensatory -= days;
                    break;
                case LeaveType.OfficialHoliday:
                    if (balance.OfficialHoliday < days) return false;
                    balance.OfficialHoliday -= days;
                    break;
                case LeaveType.Exam:
                    if (balance.Exam < days) return false;
                    balance.Exam -= days;
                    break;
                default:
                    return false;
            }

            balance.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ForceDeductBalanceAsync(int employeeId, LeaveType leaveType, int days)
        {
            var balance = await _context.HROffDayBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId);

            if (balance == null)
                return false;

            switch (leaveType)
            {
                case LeaveType.Annual: balance.Annual -= days; break;
                case LeaveType.Casual: balance.Casual -= days; break;
                case LeaveType.Sick: balance.Sick -= days; break;
                case LeaveType.Hajj: balance.Hajj -= days; break;
                case LeaveType.Maternity: balance.Maternity -= days; break;
                case LeaveType.Unpaid: balance.Unpaid -= days; break;
                case LeaveType.Compensatory: balance.Compensatory -= days; break;
                case LeaveType.OfficialHoliday: balance.OfficialHoliday -= days; break;
                case LeaveType.Exam: balance.Exam -= days; break;
                default: return false;
            }

            balance.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddToBalanceAsync(int employeeId, LeaveType leaveType, int days)
        {
            var balance = await _context.HROffDayBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId);

            if (balance == null)
                return false;

            switch (leaveType)
            {
                case LeaveType.Annual: balance.Annual += days; break;
                case LeaveType.Casual: balance.Casual += days; break;
                case LeaveType.Sick: balance.Sick += days; break;
                case LeaveType.Hajj: balance.Hajj += days; break;
                case LeaveType.Maternity: balance.Maternity += days; break;
                case LeaveType.Unpaid: balance.Unpaid += days; break;
                case LeaveType.Compensatory: balance.Compensatory += days; break;
                case LeaveType.OfficialHoliday: balance.OfficialHoliday += days; break;
                case LeaveType.Exam: balance.Exam += days; break;
                default: return false;
            }

            balance.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
