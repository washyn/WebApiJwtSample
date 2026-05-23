using System;
using System.Globalization;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Invoice
{
    public class InvoiceDocument : IDocument
    {
        private readonly InvoiceModel _model;

        public InvoiceDocument(InvoiceModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.DefaultTextStyle(x => x
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken3)
                    .FontFamily("Calibri")
                );

                page.Content().Element(ComposeContent);
            });
        }

        void ComposeContent(IContainer container)
        {
            var cultureInfo = new CultureInfo("es-pe");

            container.Column(column =>
            {
                // Encabezado (Logo, Info Empresa, RUC / Nro Comprobante)
                column.Item().Row(row =>
                {
                    row.Spacing(10);

                    if (!string.IsNullOrEmpty(_model.Seller?.LogoPath))
                    {
                        row.RelativeItem(1).Image(_model.Seller.LogoPath);
                    }
                    else
                    {
                        row.RelativeItem(1).Text(string.Empty);
                    }

                    row.RelativeItem(4).Column(col =>
                    {
                        col.Spacing(3);
                        col.Item().AlignCenter().Text(_model.Seller?.Name ?? string.Empty).Medium().FontSize(12);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext1 ?? string.Empty);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext2 ?? string.Empty);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext3 ?? string.Empty);
                    });

                    row.RelativeItem(2).Text(string.Empty);

                    row.RelativeItem(3).AlignMiddle().Row(r =>
                    {
                        r.RelativeItem()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .Column(col =>
                            {
                                col.Item().AlignCenter().Text($"RUC - {_model.Details?.Ruc ?? string.Empty}");
                                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                                col.Item().AlignCenter().Text(_model.Details?.DocumentType ?? string.Empty);
                                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                                col.Item().AlignCenter().Text(_model.Details?.DocumentNumber ?? string.Empty);
                            });
                    });
                });

                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                // Información del Cliente
                column.Item().Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(3).Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Cliente: ").SemiBold();
                            text.Span(_model.Customer?.Name ?? string.Empty);
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Documento: ").SemiBold();
                            text.Span(_model.Customer?.DocumentNumber ?? string.Empty);
                        });
                    });
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Fecha emisión: ").SemiBold();
                            text.Span(_model.Details?.IssueDate.ToString("d", cultureInfo) ?? string.Empty);
                        });
                    });
                });

                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                // Tabla de Items
                column.Item().Element(ComposeTable);

                // Sección de Totales
                column.Item().Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);
                        col.Item().Text($"Son: {_model.AmountInWords ?? string.Empty}").Medium();
                    });
                    row.RelativeItem(1).Column(col =>
                    {
                        col.Item().PaddingRight(5).AlignRight().Text($"Grand total: {_model.TotalAmount:C}").SemiBold();
                        col.Item().PaddingRight(5).AlignRight().Text($"Grand total: {_model.TotalAmount:C}").SemiBold();
                    });
                });

                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                // Firma / Pie de Página Informativo (QR e información de pago)
                column.Item().Column(col =>
                {
                    if (!string.IsNullOrEmpty(_model.HashImagePath))
                    {
                        col.Item().Width(50).Image(_model.HashImagePath);
                    }

                    col.Item().Text(a =>
                    {
                        a.Span("Codigo hash: ").SemiBold();
                        a.Span(_model.HashCode ?? string.Empty);
                    });
                    col.Item().Text(a =>
                    {
                        a.Span("Condicion pago: ").SemiBold();
                        a.Span(_model.PaymentMethod ?? string.Empty);
                    });
                });
            });
        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold().FontSize(9);

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

                    header.Cell().ColumnSpan(5).PaddingTop(2).Border(0.5F).BorderColor(Colors.Grey.Darken1);
                });

                if (_model.Items != null)
                {
                    foreach (var item in _model.Items)
                    {
                        table.Cell().Element(CellStyle).Text($"{item.Index}");
                        table.Cell().Element(CellStyle).Text(item.Description ?? string.Empty);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:C}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Total:C}");

                        static IContainer CellStyle(IContainer container) => container.BorderBottom(0.5F)
                            .BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
                    }
                }
            });
        }
    }
}
