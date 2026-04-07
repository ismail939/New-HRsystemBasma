using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HRsystem.Models;
using System;
using System.Collections.Generic;
using HRsystem.ViewModels;

namespace HRsystem.Reports
{
    public class DailyReport : IDocument
    {
        [Obsolete]
        public void Compose(IDocumentContainer container)
        {
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
                            col.Item().Text("القوات البحرية").FontSize(14).Bold();
                            col.Item().PaddingTop(5);
                            col.Item().Text("دار الاسطول الجزيرة").FontSize(14).Bold();
                        });

                        // Left logo
                        row.ConstantItem(80)
                           .Image("wwwroot/images/logo2hh.png", ImageScaling.FitArea);
                    });

                    headerCol.Item()
                        .PaddingTop(20)
                        .AlignCenter()
                        .Text(text => text.Span("تقرير يومي لكل الأقسام").Bold().FontSize(13));
                });

                page.Content()
                    .Padding(5)
                    .ContentFromRightToLeft()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
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
                            columns.RelativeColumn(2);
                        });

                        // Header row
                        string[] headers = {
                            "الاسم", "بدء الشيفت", "نهاية الشيفت","معاد الحضور",
                            "معاد الانصراف", "عدد ساعات العمل", "دقائق التأخير",
                            "ساعات العمل الاضافي", "دقائق الخروج المبكر",
                            "الحالة", "ملاحظات"
                        };

                        table.Header(header =>
                        {
                            foreach (var h in headers)
                            {
                                header.Cell().Element(cell =>
                                    cell.Border(1)
                                        .Padding(1)
                                        .AlignCenter()
                                        .AlignMiddle()
                                        .Text(text => text.Span(h).Bold())
                                );
                            }
                        });

                        int color = 0;

                        // Data rows (dummy rows for now)
                        for (int i = 0; i < 2; i++)
                        {
                            var bgColor = color % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White;

                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            table.Cell().Background(bgColor).Element(c => c.Border(1).Padding(2).AlignCenter().AlignMiddle().Text(""));
                            
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

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}