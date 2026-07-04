using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HRsystem.Models;
using System;
using System.Collections.Generic;
using HRsystem.ViewModels;


namespace HRsystem.Reports
{
    public class EmployeeReport : IDocument
    {
        public List<EmployeeViewModel> Employees { get; set; }

        public EmployeeReport(List<EmployeeViewModel> employees)
        {
            Employees = employees;
        }

        public void Compose(IDocumentContainer container)
        {
            int rowsPerPage = 15; // adjust based on row height and page size
            int totalRows = Employees.Count;

            for (int pageIndex = 0; pageIndex < Math.Ceiling(totalRows / (double)rowsPerPage); pageIndex++)
            {
                var pageEmployees = Employees.Skip(pageIndex * rowsPerPage).Take(rowsPerPage).ToList();

                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x
                        .FontSize(7)
                        .FontColor(Colors.Black)
                    );
                    page.Background().Element(bg =>
                        {
                            bg.Padding(10)
                            .Border(2)
                            .BorderColor(Colors.Black);
                        });
                    page.Header().Column(headerCol =>
                        {
                            headerCol.Item().ContentFromRightToLeft().Row(row =>
                            {
                                // Right text
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().PaddingTop(20);
                                    col.Item().Text("إدارة الموارد البشرية").FontSize(14).Bold();
                              
                                });

                                // Left logo
                                row.ConstantItem(80).Image("wwwroot/images/logo2hh.png", ImageScaling.FitArea);
                            });
                            headerCol.Item().PaddingTop(20).AlignCenter().Text(text => text.Span("كشف بجميع الموظفين").Bold().FontSize(13));
;
                        });
                    

                    page.Content().Padding(5)
                        .ContentFromRightToLeft()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Header row
                            string[] headers = { "اسم الموظف", "الرقم القومي", "رقم الهاتف","العنوان", "الحالة الاجتماعية",
                    "الديانة", "تاريخ الميلاد", "رقم التأمين", "تاريخ التعيين",
                    "تاريخ نهاية الخدمة", "القسم", "الوظيفة", "نوع العقد" };

                            table.Header(header =>
                            {
                                foreach (var h in headers)
                                    header.Cell().Element(cell => cell.Border(1).Padding(1).AlignCenter().AlignMiddle()
                                        .Text(text => text.Span(h).Bold()));
                            });
                            int color = 0;
                            // Data rows
                            foreach (var emp in pageEmployees)
                            {
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.Name));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.NationalId));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.PhoneNumber));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.Address));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.MarriageStatus));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.Religion));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.DateOfBirth.ToString("yyyy-MM-dd")));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.InsuranceNumber ?? ""));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.HireDate.ToString("yyyy-MM-dd")));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.EndDate.HasValue ? emp.EndDate.Value.ToString("yyyy-MM-dd") : ""));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.Department));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.JobName));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter().AlignMiddle()
                                    .Text(emp.ContractType));
                                color++;
                            }
                        });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated by HR System ");
                        text.Span(DateTime.Now.ToString("g"));
                    });
                });
            }

        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}
