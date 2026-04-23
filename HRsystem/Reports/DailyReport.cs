using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HRsystem.ViewModels;

public class DailyReport : IDocument
{
    private readonly DailyReportVM _data;

    public DailyReport(DailyReportVM data)
    {
        _data = data;
    }

    [Obsolete]
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9));

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
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("القوات البحرية").FontSize(14).Bold();
                        col.Item().Text("دار الاسطول الجزيرة").FontSize(14).Bold();
                    });

                    row.ConstantItem(80)
                        .Height(50)
                        .Image(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo2hh.png"),
                               ImageScaling.FitArea);
                });

                headerCol.Item()
                    .AlignCenter()
                    .Text($"تقرير يوم {_data.Day:yyyy-MM-dd}")
                    .Bold()
                    .FontSize(13);
            });
            page.Content()
    .Padding(5)
    .ContentFromRightToLeft()
    .Column(col =>
    {
        var grouped = _data.Rows
            .GroupBy(x => x.DepartmentName)
            .OrderBy(g => g.Key);
        bool first = true;
        foreach (var departmentGroup in grouped)
        {
            if (!first)
                col.Item().PageBreak();

            first = false;
            // 🟢 Department Header
            col.Item()
                .PaddingTop(10)
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .Text(departmentGroup.Key ?? "بدون قسم")
                .Bold()
                .FontSize(11);

            // 🟢 Table per department
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // الاسم
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                string[] headers =
                {
                    "الاسم",
                    "بدء الشيفت",
                    "نهاية الشيفت",
                    "الحضور",
                    "الانصراف",
                    "ساعات العمل",
                    "التأخير",
                    "العمل الاضافي",
                    "الخروج المبكر",
                    "الحالة",
                    "ملاحظات"
                };

                table.Header(header =>
                {
                    foreach (var h in headers)
                    {
                        header.Cell()
                            .Border(0.5f)
                            .Padding(2)
                            .AlignCenter()
                            .Text(h)
                            .Bold();
                    }
                });

                int color = 0;

                foreach (var row in departmentGroup)
                {
                    var bg = color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White;

                    void Cell(string value) =>
                        table.Cell()
                            .Background(bg)
                            .Border(0.5f)
                            .Padding(2)
                            .AlignCenter()
                            .Text(value ?? "");

                    Cell(row.EmployeeName);
                    Cell(row.ShiftStart ?? "");
                    Cell(row.ShiftEnd ?? "");
                    Cell(row.ArrivalTime ?? "");
                    Cell(row.DepartureTime ?? "");
                    Cell(row.TotalHours ?? "");
                    Cell(row.LateMinutes ?? "");
                    Cell(row.OvertimeMinutes ?? "");
                    Cell(row.EarlyLeaveMinutes ?? "");
                    Cell(row.Status ?? "");
                    Cell(row.Notes ?? "");

                    color++;
                }
            });
        }
    });
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Generated by HR System ");
                text.Span(DateTime.Now.ToString("g"));
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}