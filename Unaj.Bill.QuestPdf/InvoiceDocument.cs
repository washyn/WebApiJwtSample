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
                        row.RelativeItem().Column(a => a.Item()
                            .Width(40).Image("opera.png"));
                    }
                    else
                    {
                        row.RelativeItem().Column(a => a.Item()
                            .Text(string.Empty));
                    }

                    row.RelativeItem(5).Column(col =>
                    {
                        col.Item().AlignCenter().Text(_model.Seller?.Name ?? string.Empty).Medium().FontSize(12);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext1 ?? string.Empty);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext2 ?? string.Empty);
                    });

                    row.RelativeItem().Text(string.Empty);

                    row.RelativeItem(3).AlignMiddle().Row(r =>
                    {
                        r.RelativeItem()
                            .Border(0.5F)
                            .BorderColor(Colors.Grey.Darken1)
                            .PaddingVertical(5)
                            .Column(col =>
                            {
                                col.Spacing(3);
                                col.Item().AlignCenter().Text(_model.Details?.DocumentType);
                                // col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                                col.Item().AlignCenter().Text(_model.Details?.DocumentNumber ?? string.Empty);
                            });
                    });
                });

                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                // Información del Cliente
                column.Item().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span("Cliente: ").SemiBold();
                        text.Span(_model.Customer?.Name ?? string.Empty);
                    });

                    col.Item().Text(text =>
                    {
                        text.Span("Método de Pago: ").SemiBold();
                        text.Span("Billetera Digital Bipay");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Fecha de Operación: ").SemiBold();
                        text.Span(_model.Details?.IssueDate.ToString("D"));
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Celular (Bipay): ").SemiBold();
                        text.Span("997 *** 563");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("ID de Transacción Bipay: ").SemiBold();
                        text.Span("235614");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Nro. de Comprobante: ").SemiBold();
                        text.Span("UNAJ-2026-0A54AEC8");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("ID de Solicitud UNAJ: ").SemiBold();
                        text.Span("0a54aec8-d872-8e8e-48f4-3a212ed15e23");
                    });
                });


                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);


                column.Item().Element(ComposeTable);


                column.Item().PaddingBottom(5).Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);
                        col.Item().Text($"Son: {_model.AmountInWords ?? string.Empty}").Medium();
                    });
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text($"Importe total: {_model.TotalAmount:C}")
                            .SemiBold();
                    });
                });


                column.Item().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
                {
                    row.RelativeItem(10).Column(a =>
                    {
                        a.Item().Text(text =>
                        {
                            text.Span("AUTENTICIDAD DEL DOCUMENTO").FontSize(8);
                        });

                        a.Item().Text(text =>
                        {
                            text.Span(
                                    "Este comprobante fue generado automáticamente tras confirmar el pago en Bipay. Para verificar su autenticidad, utilice el código de verificación o escanee el código QR.")
                                .FontSize(8);
                        });

                        a.Item().Text(text =>
                        {
                            text.Span("Código de Verificación: ").FontSize(8);
                            text.Span("EA828F8E361C").FontSize(8);
                        });

                        a.Item().Text(text =>
                        {
                            text.Span("URL de Verificación: ").FontSize(8);
                            text.Span(
                                    "https://pagos.unaj.edu.pe/verificar?r=0a54aec8-d872-8e8e-48f4-3a212ed15e23&c=EA828F8E361C")
                                .FontSize(8);
                        });
                    });
                    row.RelativeItem(2)
                        .Column(a =>
                        {
                            a.Item()
                                .PaddingHorizontal(10).Width(50).Image("opera.png");
                        });
                });
                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);
                column.Item().Column(a =>
                {
                    a.Spacing(5);
                    a.Item().Text(text =>
                    {
                        text.Span(_model.Message ?? string.Empty).Italic().FontSize(7);
                    });
                    a.Item().Text(text =>
                    {
                        text.Span(_model.MessageWarning ?? string.Empty).FontSize(7);
                    });
                });


                column.Item().PaddingVertical(5).Column(col =>
                {
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Emitido electrónicamente por la UNAJ — ")
                            .FontFamily("Calibri")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);

                        text.Span("UNAJ-2026-0A54AEC8")
                            .FontFamily("Calibri")
                            .FontSize(8)
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken3);
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
                    columns.ConstantColumn(35);
                    columns.RelativeColumn(3);

                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("Código").Style(headerStyle);
                    header.Cell().Text("Concepto").Style(headerStyle);

                    header.Cell().AlignRight().Text("Importe").Style(headerStyle);

                    header.Cell().ColumnSpan(3).PaddingTop(2).Border(0.5F).BorderColor(Colors.Grey.Darken1);
                });

                if (_model.Items != null)
                {
                    foreach (var item in _model.Items)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Index}");
                        table.Cell().Element(CellStyle).Text(item.Description ?? string.Empty);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Total:C}");

                        static IContainer CellStyle(IContainer container) => container.BorderBottom(0.5F)
                            .BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
                    }
                }
            });
        }
    }
}
