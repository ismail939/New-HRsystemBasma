using HRsystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

public class EmployeeReportDHVM : IDocument
{
    public HREmployeeDHVM EmployeeObject { get; set; }

    public EmployeeReportDHVM(HREmployeeDHVM employeeObject)
    {
        EmployeeObject = employeeObject;
    }

    [Obsolete]
    public void Compose(IDocumentContainer container)
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
                col.Item().AlignCenter().Text($"تقرير موظف: {EmployeeObject.EmployeeName}")
                    .FontSize(14).Bold();

                col.Item().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                    });

                    void Row(string title, string value)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text(title);
                        table.Cell().Element(CellStyle).AlignCenter().Text(value);
                    }

                    Row("عدد ايام الحضور", EmployeeObject.EntryDaysCount.ToString());
                    Row("عدد ساعات العمل", EmployeeObject.LeavesCount.ToString());
                    Row("عدد الغيابات", EmployeeObject.AbsencesCount.ToString());
                    Row("عدد ايام الاجازة", EmployeeObject.LeavesCount.ToString());
                    Row("عدد ايام الراحات", EmployeeObject.LeavesCount.ToString());
                    Row("عدد الجزاءات", EmployeeObject.LeavesCount.ToString());
                    Row("التأخير عن بداية الشيفت", EmployeeObject.LeavesCount.ToString());
                    Row("مجموع دقائق الخروج المبكر", EmployeeObject.LeavesCount.ToString());
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
                col.Item().Text("التقييم العام للموظف:").AlignCenter();
                col.Item().PaddingBottom(5);
                col.Item().Height(20).Element(bar =>
                {
                    float percent = 80;   // your value 0–100

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

                // --------------------------------------
                // Penalty table
                // --------------------------------------
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

                    foreach (var penalty in EmployeeObject.Penalty)
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

                // --------------------------------------
                // Leaves table
                // --------------------------------------
                // col.Item().PaddingTop(20);
                // col.Item().Text($"عدد الاجازات: {EmployeeObject.LeavesCount}");

                // col.Item().PaddingTop(10);
                // col.Item().Table(table =>
                // {
                //     table.ColumnsDefinition(columns =>
                //     {
                //         columns.RelativeColumn();
                //         columns.RelativeColumn();
                //     });

                //     table.Header(header =>
                //     {
                //         header.Cell().Background(Colors.Grey.Lighten2)
                //             .Border(1).Padding(5)
                //             .Text("النوع").Bold().AlignCenter();

                //         header.Cell().Background(Colors.Grey.Lighten2)
                //             .Border(1).Padding(5)
                //             .Text("التاريخ").Bold().AlignCenter();
                //     });

                //     foreach (var leave in EmployeeObject.Leaves)
                //     {
                //         table.Cell().Border(1).Padding(5)
                //             .Text(leave.Type).AlignCenter();

                //         table.Cell().Border(1).Padding(5)
                //             .Text($"{leave.Date:dd-MM-yyyy}").AlignCenter();
                //     }
                // }
                // );

                // // --------------------------------------
                // // Penalty count
                // // --------------------------------------
                // col.Item().PaddingTop(20);
                // col.Item().Text($"عدد الجزاءات: {EmployeeObject.PenaltyCount}");


            });

            // ========= FOOTER =========
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Generated By HR • ");
                text.Span(DateTime.Now.ToString("g"));
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}
