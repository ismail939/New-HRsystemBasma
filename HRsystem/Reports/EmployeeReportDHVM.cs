using HRsystem.Models;
using HRsystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

public class EmployeeReportDHVM : IDocument
{
    public List<HREmployeeDHVM> EmployeeReports { get; set; }

    public EmployeeReportDHVM(List<HREmployeeDHVM> EmployeeReports)
    {
        this.EmployeeReports = EmployeeReports;
    }

    [Obsolete]
    public void Compose(IDocumentContainer container)
    {
        foreach (var EmployeeReport in EmployeeReports)
        {

            container.Page(page =>
            {
                page.Margin(30);

                // ========= PAGE BORDER (FULL PAGE) =========
                page.Background().Element(bg =>
                {
                    bg.Padding(10)
                    .Border(2)
                    .BorderColor(Colors.Black);
                });

                // ========= DEFAULT TEXT + RTL =========
                page.DefaultTextStyle(x => x
                    .FontSize(12)
                    .FontColor(Colors.Black)
                    .DirectionFromRightToLeft()
                );

                // ========= HEADER =========
                page.Header().Column(headerCol =>
                {
                    headerCol.Item().ContentFromRightToLeft().Row(row =>
                    {
                        // Right text
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().PaddingTop(20);
                            col.Item().Text("القوات البحرية").FontSize(14).Bold();
                            col.Item().PaddingTop(5);
                            col.Item().Text("دار الاسطول الجزيرة").FontSize(14).Bold();
                        });

                        // Left logo
                        row.ConstantItem(80).Image("wwwroot/images/logo2hh.png", ImageScaling.FitArea);
                    });
                });

                // ========= CONTENT (NO BORDER ANYMORE) =========
                page.Content().Padding(10).ContentFromRightToLeft().Column(col =>
                {
                    col.Item().AlignCenter().Text($"تقرير موظف: {EmployeeReport.EmployeeName}")
                        .FontSize(14).Bold();

                    col.Item().PaddingTop(10).Text($"الفترة من: {EmployeeReport.ReportStartDate:dd-MM-yyyy} إلي: {EmployeeReport.ReportEndDate:dd-MM-yyyy}").AlignCenter();
                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        void Row(string title, string value, bool evenRow)
                        {
                            if (evenRow)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Element(CellStyle).AlignCenter().Text(title);
                                table.Cell().Background(Colors.Grey.Lighten3).Element(CellStyle).AlignCenter().Text(value);
                            }
                            else
                            {
                                table.Cell().Element(CellStyle).AlignCenter().Text(title);
                                table.Cell().Element(CellStyle).AlignCenter().Text(value);
                            }
                        }

                        Row("عدد ايام الحضور", EmployeeReport.EntryDaysCount.ToString(), true);
                        Row("عدد ساعات العمل", EmployeeReport.TotalWorkHours.ToString(), false);
                        Row("عدد الغيابات", EmployeeReport.AbsencesCount.ToString(), true);
                        Row("عدد ايام الاجازة", EmployeeReport.LeavesCount.ToString(), false);
                        Row("عدد ايام المرضي", EmployeeReport.IllsCount.ToString(), true);
                        Row("عدد ايام الراحات", EmployeeReport.OffsCount.ToString(), false);
                        Row("عدد الجزاءات", EmployeeReport.PenaltyCount.ToString(), true);
                        Row("التأخير عن بداية الشيفت", EmployeeReport.TotalLateMinutes.ToString(), false);
                        Row("مجموع دقائق الخروج المبكر", EmployeeReport.TotalEarlyLeaveMinutes.ToString(), true);
                        Row("مجموع ساعات العمل الإضافي", (EmployeeReport.TotalOvertimeMinutes / 60.0).ToString("F2"), false);
                    });

                    // شكل الخلايا
                    IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Border(1)
                            .BorderColor("#000") // خليها أسود لو عايز أوضح
                            .PaddingVertical(6)
                            .PaddingHorizontal(10)
                            .AlignMiddle()   // vertical center
                            .AlignCenter();  // horizontal center
                    }
                    col.Item().PaddingTop(10);
                    if(EmployeeReport.TotalRate > 0)
                    {
                        col.Item().Text("التقييم العام للموظف:").AlignCenter();
                        col.Item().PaddingBottom(5);
                        col.Item().Height(20).Element(bar =>
                        {
                            float percent = (float)EmployeeReport.TotalRate*20;   // your value 0–100
                            Console.WriteLine($"⭐TotalRate: {EmployeeReport.TotalRate}, Percent: {percent}");
                            bar.Row(row =>
                            {
                                // Filled part
                                row.RelativeItem(percent)
                                .Background(Colors.Green.Medium)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text($"{percent}%").FontColor(Colors.White);

                                // Empty part
                                row.RelativeItem(100 - percent)
                                .Background(Colors.Grey.Lighten3);
                            });
                        });
                        
                    }

                    // --------------------------------------
                    // Penalty table
                    // --------------------------------------
                    if(EmployeeReport.PenaltyCount == 0)
                    {
                        return; // ما فيش جزاءات، ما نعرضش الجدول
                    }
                    col.Item().PaddingTop(20);
                    col.Item().Text($" الجزاءات:").FontSize(14).AlignCenter();
                    col.Item().PaddingTop(10);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2)
                                .Border(1).Padding(5)
                                .Text("التاريخ").Bold().AlignCenter();

                            header.Cell().Background(Colors.Grey.Lighten2)
                                .Border(1).Padding(5)
                                .Text("السبب").Bold().AlignCenter();

                            header.Cell().Background(Colors.Grey.Lighten2)
                                .Border(1).Padding(5)
                                .Text("القرار").Bold().AlignCenter();
                        });

                        foreach (var penalty in EmployeeReport.Penalty)
                        {
                            table.Cell().Border(1).Padding(5)
                                .Text($"{penalty.Date:dd-MM-yyyy}")
                                .AlignCenter();
                            table.Cell().Border(1).Padding(5)
                                .Text(penalty.Reason)
                                .AlignCenter();
                            table.Cell().Border(1).Padding(5)
                                .Text(penalty.Decision)
                                .AlignCenter();
                        }
                    });
                });

                // ========= FOOTER =========
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated By HR • ");
                    text.Span(DateTime.Now.ToString("g"));
                });
            }).Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape()); // 👈 Landscape
                        page.Margin(30);
                        page.Background().Element(bg =>
                        {
                            bg.Padding(10)
                            .Border(2)
                            .BorderColor(Colors.Black);
                        });
                        page.DefaultTextStyle(x => x
                            .FontSize(12)
                            .FontColor(Colors.Black)
                            .DirectionFromRightToLeft()
                        );
                        page.Header().Column(headerCol =>
                        {
                            headerCol.Item().ContentFromRightToLeft().Row(row =>
                            {
                                // Right text
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().PaddingTop(20);
                                    col.Item().Text("القوات البحرية").FontSize(14).Bold();
                                    col.Item().PaddingTop(5);
                                    col.Item().Text("دار الاسطول الجزيرة").FontSize(14).Bold();
                                });

                                // Left logo
                                row.ConstantItem(80).Image("wwwroot/images/logo2hh.png", ImageScaling.FitArea);
                            });
                        });
                        page.Content().Padding(20).ContentFromRightToLeft().Column(col =>
                        {
                            if(EmployeeReport.BasmaList == null || EmployeeReport.BasmaList.Count == 0)
                            {
                                col.Item().Text("لا توجد بيانات حضور وانصراف لهذا الموظف في الفترة المحددة.").FontSize(12).AlignCenter();
                                return;
                            }
                            col.Item().Text($"الحضور والانصراف من {EmployeeReport.ReportStartDate:dd-MM-yyyy} إلي {EmployeeReport.ReportEndDate:dd-MM-yyyy}   للموظف: {EmployeeReport.EmployeeName}")
                                .FontSize(14).AlignCenter();

                            col.Item().PaddingTop(10);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    IContainer HeaderCellStyle(IContainer container)
                                    {
                                        return container
                                            .Background(Colors.Grey.Lighten2)
                                            .Border(1)
                                            .AlignCenter()
                                            .AlignMiddle();
                                    }

                                    void HeaderCell(string text)
                                    {
                                        header.Cell()
                                            .Element(HeaderCellStyle)
                                            .Padding(2)
                                            .Text(text)
                                            .FontSize(8).Bold();
                                    }

                                    HeaderCell("التاريخ");
                                    HeaderCell("الاسم");
                                    HeaderCell("بدء الشيفت");
                                    HeaderCell("انتهاء الشيفت");
                                    HeaderCell("الحضور");
                                    HeaderCell("الانصراف");
                                    HeaderCell("عدد الساعات");
                                    HeaderCell("التأخير");
                                    HeaderCell("دقائق الخروج المبكر");
                                    HeaderCell("الحالة");
                                    HeaderCell("الملاحظات");
                                });
                            int color = 1;
                            foreach (var basma in EmployeeReport.BasmaList)
                            {
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(2).AlignCenter()
                                    .Text($"{basma.DayDate:dd-MM-yyyy}").FontSize(8));

                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(EmployeeReport.EmployeeName).FontSize(8));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.ShiftStart.HasValue ? basma.ShiftStart.Value.ToString("HH:mm") : "-").FontSize(8));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.ShiftEnd.HasValue ? basma.ShiftEnd.Value.ToString("HH:mm") : "-").FontSize(8));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.Arrival.HasValue ? basma.Arrival.Value.ToString("HH:mm") : "-").FontSize(8)); 
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter())
                                    .Text(basma.Departure.HasValue ? basma.Departure.Value.ToString("HH:mm") : "-").FontSize(8); 
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.TotalHours.HasValue ? basma.TotalHours.Value.ToString("0.##") : "-").FontSize(8));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.LateMinutes.HasValue ? basma.LateMinutes.Value.ToString() : "-").FontSize(8));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.EarlyLeaveMinutes.HasValue ? basma.EarlyLeaveMinutes.Value.ToString() : "-").FontSize(8));
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.OvertimeMinutes.HasValue ? basma.OvertimeMinutes.Value.ToString() : "-").FontSize(8));
                                if(basma.Status == 1)
                                {
                                    table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text("حضور").FontSize(8));
                                }
                                else if(basma.Status == 2)
                                {
                                    var leave = EmployeeReport.Leaves.Find(l => l.Date.Date == basma.DayDate.Date);
                                    if(leave != null)
                                        {
                                             table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                             .Text(leave.Type).FontSize(8));
                                        }
                                    var off = EmployeeReport.Offs.Find(l => l.Date.Date == basma.DayDate.Date);
                                    if(off != null)                                        {
                                             table.Cell().Background(color%2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                             .Text(off.Type).FontSize(8));
                                        }

                                }
                                else if(basma.Status == 0)
                                {
                                    table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text("غياب").FontSize(8));
                                }
                                table.Cell().Background(color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White).Element(cell => cell.Border(1).Padding(5).AlignCenter()
                                    .Text(basma.Notes ?? "-").FontSize(8));
                                color++;
                                
                            }  
                            });
                        });
                        // ========= FOOTER =========
                        page.Footer().AlignCenter().Text(text =>
                    {
                    text.Span("Generated By HR • ");
                    text.Span(DateTime.Now.ToString("g"));
                });
                    });

        }
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}
