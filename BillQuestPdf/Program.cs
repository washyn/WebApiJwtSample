﻿using System;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Invoice
{
    public class InvoiceDocument : IDocument
    {
        public InvoiceDocument(object model)
        {
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.DefaultTextStyle(x => x
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken3)
                        .FontFamily("Calibri")
                    );

                    // page.Header().Element(ComposeHeader);

                    page.Content().Element(ComposeContent);

                    // page.Footer().AlignCenter().Text(text =>
                    // {
                    //     text.CurrentPageNumber();
                    //     text.Span(" / ");
                    //     text.TotalPages();
                    // });
                });
        }

        void ComposeContent(IContainer container)
        {
            // CultureInfo.CurrentCulture.Name
            var cultureInfo = new CultureInfo("es-pe");
            // .PaddingVertical(40)
            container
                // .Background(Colors.Green.Lighten5)
                .Column(column =>
                {
                    // column.Spacing(20);

                    // column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    // TODO: para la info del cliente crear 3 columnas...

                    column.Item().Row(row =>
                    {
                        row.Spacing(10);
                        // TODO: add only if exists image
                        row.RelativeItem(1)
                            // .Border(1)
                            // .Background(Colors.Grey.Lighten1)
                            .Image("opera.png");
                        row.RelativeItem(4)
                            .Column(col =>
                            {
                                col.Spacing(-3);
                                col.Item()
                                    .AlignCenter()
                                    .Text("Empresa S.A.")
                                    .Medium()
                                    .FontSize(12);
                                        
                                col.Item()
                                    .AlignCenter()
                                    .Text("Empresa S.A. efhjfsdjkef");
                                col.Item()
                                    .AlignCenter()
                                    .Text("Empresa S.A. 98734589745389");
                                col.Item()
                                    .AlignCenter()
                                    .Text("Empresa S.A. 8448");
                            });
                        
                        row.RelativeItem(2)
                            .Text(String.Empty);
                        row.RelativeItem(3)
                            .AlignMiddle()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Column(col =>
                                    {
                                        col.Item()
                                            .AlignCenter()
                                            .Text("RUC - 00000000");
                                        col.Item()
                                            .LineHorizontal(1)
                                            .LineColor(Colors.Grey.Lighten1);
                                        col.Item()
                                            .AlignCenter()
                                            .Text("COMPROBANTE   ELECTRONICO");
                                        col.Item()
                                            .LineHorizontal(1)
                                            .LineColor(Colors.Grey.Lighten1);
                                        col.Item()
                                            .AlignCenter()
                                            .Text("B00-345788475");
                                    });
                            });
                    });

                    column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                    column.Item().Row(row =>
                    {
                        row.Spacing(10);
                        row.RelativeItem(3)
                            .Column(col =>
                            {
                                col.Item().Text(text =>
                                {
                                    text.Span("Cliente: ").SemiBold();
                                    text.Span("Chester Chester Chester");
                                });
                                col.Item().Text(text =>
                                {
                                    text.Span("Documento: ").SemiBold();
                                    text.Span("71449257");
                                });
                            });
                        row.RelativeItem(2)
                            .Column(col =>
                            {
                                col.Item().Text(text =>
                                {
                                    text.Span("Fecha emisión: ").SemiBold();
                                    text.Span(DateTime.Now.ToString("d", cultureInfo));
                                });
                            });
                    });
                    
                    column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                    column.Item().Element(ComposeTable);
                    var totalPrice = 546;

                    column.Item().Row(row =>
                    {
                        row.Spacing(10);
                        row.RelativeItem(2).Column(col =>
                        {
                            col.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);
                            col.Item().Text($"Son: Mucho dineroo").Medium();
                        });
                        row.RelativeItem(1).Column(col =>
                        {
                            col.Item().PaddingRight(5).AlignRight().Text($"Grand total: {totalPrice:C}").SemiBold();
                            col.Item().PaddingRight(5).AlignRight().Text($"Grand total: {totalPrice:C}").SemiBold();
                        });
                    });
                    
                    column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                    column.Item().Column(col =>
                    {
                        col.Item().Width(50).Image("opera.png");
                        col.Item().Text(a =>
                        {
                            a.Span("Codigo hash:").SemiBold();
                            a.Span("hjksdfhjkfdjhksdfjh5");
                        });
                        col.Item().Text(a =>
                        {
                            a.Span("Condicion pago:").SemiBold();
                            a.Span("Contado");
                        });
                    });
                });
        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold()
                .FontSize(9);

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("#");
                    header.Cell().Text("Producto").Style(headerStyle);
                    header.Cell().AlignRight().Text("P.U.").Style(headerStyle);
                    header.Cell().AlignRight().Text("C.").Style(headerStyle);
                    header.Cell().AlignRight().Text("Total").Style(headerStyle);

                    header.Cell().ColumnSpan(5).PaddingTop(2).BorderBottom(0.5F).BorderColor(Colors.Grey.Darken1);
                });

                for (var i = 1; i <= 10; i++)
                {
                    table.Cell().Element(CellStyle).Text($"{i}");
                    table.Cell().Element(CellStyle).Text(i);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{i:C}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{i}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{i:C}");

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(0.5F)
                        .BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
                }
            });
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Please make sure that you are eligible to use the Community license.
            // To learn more about the QuestPDF licensing, please visit:
            // https://www.questpdf.com/pricing.html
            Settings.License = LicenseType.Community;

            // For documentation and implementation details, please visit:
            // https://www.questpdf.com/documentation/getting-started.html
            var model = new { };
            var document = new InvoiceDocument(model);

            // Generate PDF file and show it in the default viewer
            document.GeneratePdfAndShow();

            // Or open the QuestPDF Previewer and experiment with the document's design
            // in real-time without recompilation after each code change
            // https://www.questpdf.com/document-previewer.html
            //document.ShowInPreviewer();
        }
    }
}