using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HRsystem.Models;
using System;
using System.Collections.Generic;

namespace HRsystem.Reports
{
    public class EmployeeReport : IDocument
    {
        public List<HREmployee> Employees { get; set; }

        public EmployeeReport(List<HREmployee> employees)
        {
            Employees = employees;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape()); // Landscape
                page.Margin(20);

                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));
                page.DefaultTextStyle(x =>
                    x.FontSize(10)
                    .FontColor(Colors.Black)
                    .DirectionFromRightToLeft()
                );

                page.Header()
                    .Text("كشف الموظفين")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();

                page.Content()
                    .Padding(5)
                    .ContentFromRightToLeft()
                    .Table(table =>
                    {
                        // Define columns (one column per property)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Name
                            columns.RelativeColumn(); // NationalId
                            columns.RelativeColumn(); // PhoneNumber
                            columns.RelativeColumn(); // MarriageStatus
                            columns.RelativeColumn(); // Religion
                            columns.RelativeColumn(); // DateOfBirth
                            columns.RelativeColumn(); // InsuranceNumber
                            columns.RelativeColumn(); // HireDate
                            columns.RelativeColumn(); // EndDate
                            columns.RelativeColumn(); // Department
                            columns.RelativeColumn(); // JobName
                            columns.RelativeColumn(); // ContractType
                            columns.RelativeColumn(); // LeaveReason
                            columns.RelativeColumn(); // BasmaId
                        });

                        // Header row
                        table.Header(header =>
                        {
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("اسم الموظف").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("الرقم القومي").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("رقم الهاتف").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("الحالة الاجتماعية").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("الدين").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("تاريخ الميلاد").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("رقم التأمين").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("تاريخ التعيين").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("تاريخ نهاية الخدمة").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("القسم").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("الوظيفة").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("نوع العقد").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("سبب الإجازة").Bold());
                            header.Cell().Element(cell => cell.Border(1).Padding(3).Text("BasmaId").Bold());
                        });

                        // Data rows
                        foreach (var emp in Employees)
                        {
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.Name));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.NationalId));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.PhoneNumber));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.MarriageStatus));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.Religion));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.DateOfBirth.ToString("yyyy-MM-dd")));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.InsuranceNumber ?? ""));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.HireDate.ToString("yyyy-MM-dd")));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.EndDate.HasValue ? emp.EndDate.Value.ToString("yyyy-MM-dd") : ""));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.Department));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.JobName));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.ContractType));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.LeaveReason ?? ""));
                            table.Cell().Element(cell => cell.Border(1).Padding(3).Text(emp.BasmaId.HasValue ? emp.BasmaId.Value.ToString() : ""));
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generated by HR System ");
                        text.Span(DateTime.Now.ToString("g"));
                    });
            });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}
