using HRsystem.Models;
using HRsystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRsystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Only seed if no notifications exist
            if (await context.Notifications.AnyAsync())
                return;

            var now = DateTime.UtcNow;

            // Get users by role
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            var hrUsers = await context.Users.Where(u => u.Role == "HR").ToListAsync();
            var employeeUsers = await context.Users.Where(u => u.Role == "Employee").ToListAsync();

            if (adminUser == null || !hrUsers.Any() || !employeeUsers.Any())
                return;

            // ===== Employee Seed Data =====
            var empUser = employeeUsers.First();
            var empEmployee = await context.HREmployees.FirstOrDefaultAsync(e => e.NationalId == empUser.Username);

            if (empEmployee != null)
            {
                // 1. Pending Casual Leave
                var pendingRequest = new Request
                {
                    EmployeeId = empEmployee.Id,
                    CreatedByUserId = empUser.Id,
                    RequestType = RequestType.Leave,
                    LeaveType = LeaveType.Casual,
                    Status = RequestStatus.Pending,
                    Reason = "ظرف عائلي طارئ",
                    CreatedAt = now.AddMinutes(-5)
                };
                context.Requests.Add(pendingRequest);
                await context.SaveChangesAsync();

                // Notification for pending request
                context.Notifications.Add(new Notification
                {
                    UserId = empUser.Id,
                    Title = "تم تقديم طلب إجازة",
                    Message = "تم تقديم طلب إجازة عارضة بنجاح. في انتظار الموافقة.",
                    NotificationType = NotificationType.LeaveRequest,
                    IsRead = false,
                    CreatedAt = now.AddMinutes(-5),
                    RelatedRequestId = pendingRequest.RequestId
                });

                // 2. Approved Sick Leave
                var approvedRequest = new Request
                {
                    EmployeeId = empEmployee.Id,
                    CreatedByUserId = empUser.Id,
                    RequestType = RequestType.Leave,
                    LeaveType = LeaveType.Sick,
                    Status = RequestStatus.Approved,
                    Reason = "وعكة صحية",
                    CreatedAt = now.AddDays(-2),
                    RespondedAt = now.AddDays(-2).AddHours(3),
                    RespondedByUserId = hrUsers.First().Id,
                    ResponseMessage = "تمت الموافقة، أتمنى لك الشفاء العاجل"
                };
                context.Requests.Add(approvedRequest);
                await context.SaveChangesAsync();

                context.Notifications.Add(new Notification
                {
                    UserId = empUser.Id,
                    Title = "تم الموافقة على طلب الإجازة",
                    Message = "تمت الموافقة على طلب إجازة مرضية الخاص بك.",
                    NotificationType = NotificationType.LeaveApproved,
                    IsRead = true,
                    CreatedAt = now.AddDays(-2).AddHours(3),
                    RelatedRequestId = approvedRequest.RequestId
                });

                // 3. Rejected Annual Leave
                var rejectedRequest = new Request
                {
                    EmployeeId = empEmployee.Id,
                    CreatedByUserId = empUser.Id,
                    RequestType = RequestType.Leave,
                    LeaveType = LeaveType.Annual,
                    Status = RequestStatus.Rejected,
                    Reason = "رحلة عائلية",
                    CreatedAt = now.AddDays(-7),
                    RespondedAt = now.AddDays(-7).AddHours(2),
                    RespondedByUserId = hrUsers.First().Id,
                    ResponseMessage = "نأسف للرفض، لا يوجد عدد كافي من الموظفين في هذا التوقيت"
                };
                context.Requests.Add(rejectedRequest);
                await context.SaveChangesAsync();

                context.Notifications.Add(new Notification
                {
                    UserId = empUser.Id,
                    Title = "تم رفض طلب الإجازة",
                    Message = "تم رفض طلب إجازة سنوية. السبب: لا يوجد عدد كافي من الموظفين في هذا التوقيت",
                    NotificationType = NotificationType.LeaveRejected,
                    IsRead = true,
                    CreatedAt = now.AddDays(-7).AddHours(2),
                    RelatedRequestId = rejectedRequest.RequestId
                });
            }

            // ===== HR Seed Data =====
            foreach (var hr in hrUsers)
            {
                // 3 Pending Leave Requests (from different employees)
                var employees = await context.HREmployees
                    .Where(e => e.NationalId != null)
                    .Take(3)
                    .ToListAsync();

                for (int i = 0; i < employees.Count; i++)
                {
                    var leaveType = i switch
                    {
                        0 => LeaveType.Annual,
                        1 => LeaveType.Casual,
                        _ => LeaveType.Sick
                    };

                    var leaveNames = new[] { "سنوية", "عارضة", "مرضية" };
                    var reasons = new[] { "إجازة سنوية", "ظرف عائلي", "مرض" };

                    var req = new Request
                    {
                        EmployeeId = employees[i].Id,
                        CreatedByUserId = hr.Id,
                        RequestType = RequestType.Leave,
                        LeaveType = leaveType,
                        Status = RequestStatus.Pending,
                        Reason = reasons[i],
                        CreatedAt = now.AddMinutes(-30 * (i + 1))
                    };
                    context.Requests.Add(req);
                    await context.SaveChangesAsync();

                    context.Notifications.Add(new Notification
                    {
                        UserId = hr.Id,
                        Title = "طلب إجازة جديد",
                        Message = $"قام الموظف {employees[i].Name} بتقديم طلب إجازة {leaveNames[i]}.",
                        NotificationType = NotificationType.LeaveRequest,
                        IsRead = i > 0, // first one unread
                        CreatedAt = now.AddMinutes(-30 * (i + 1)),
                        RelatedRequestId = req.RequestId
                    });
                }

                // 2 Employee Late Notifications
                var lateEmployee1 = employees.FirstOrDefault();
                var lateEmployee2 = employees.Skip(1).FirstOrDefault();

                if (lateEmployee1 != null)
                {
                    context.Notifications.Add(new Notification
                    {
                        UserId = hr.Id,
                        Title = "موظف متأخر",
                        Message = $"الموظف {lateEmployee1.Name} متأخر في يوم {now.AddDays(-1):yyyy-MM-dd}.",
                        NotificationType = NotificationType.EmployeeLate,
                        IsRead = false,
                        CreatedAt = now.AddHours(-2)
                    });
                }

                if (lateEmployee2 != null)
                {
                    context.Notifications.Add(new Notification
                    {
                        UserId = hr.Id,
                        Title = "موظف متأخر",
                        Message = $"الموظف {lateEmployee2.Name} متأخر في يوم {now.AddDays(-1):yyyy-MM-dd}.",
                        NotificationType = NotificationType.EmployeeLate,
                        IsRead = true,
                        CreatedAt = now.AddHours(-4)
                    });
                }

                // 1 Missing Check-In Notification
                var absentEmployee = employees.LastOrDefault();
                if (absentEmployee != null)
                {
                    context.Notifications.Add(new Notification
                    {
                        UserId = hr.Id,
                        Title = "موظف غائب",
                        Message = $"الموظف {absentEmployee.Name} لم يسجل حضور أو انصراف في يوم {now.AddDays(-1):yyyy-MM-dd}.",
                        NotificationType = NotificationType.EmployeeAbsent,
                        IsRead = false,
                        CreatedAt = now.AddHours(-1)
                    });
                }
            }

            // ===== Admin Seed Data =====
            // Employee Removal Request
            context.Notifications.Add(new Notification
            {
                UserId = adminUser.Id,
                Title = "طلب حذف موظف",
                Message = $"قام {hrUsers.First().Username} بطلب حذف الموظف أحمد محمد.",
                NotificationType = NotificationType.EmployeeRemovalRequest,
                IsRead = false,
                CreatedAt = now.AddDays(-1)
            });

            // Department Creation Request
            context.Notifications.Add(new Notification
            {
                UserId = adminUser.Id,
                Title = "طلب إنشاء قسم",
                Message = $"قام {hrUsers.First().Username} بطلب إنشاء قسم جديد: قسم التطوير.",
                NotificationType = NotificationType.DepartmentCreationRequest,
                IsRead = false,
                CreatedAt = now.AddDays(-1).AddHours(-2)
            });

            await context.SaveChangesAsync();
        }
    }
}