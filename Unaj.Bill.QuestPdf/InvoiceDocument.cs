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
                        // row.RelativeItem().Image(_model.Seller.LogoPath);
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
                        // col.Spacing();
                        col.Item().AlignCenter().Text(_model.Seller?.Name ?? string.Empty).Medium().FontSize(12);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext1 ?? string.Empty);
                        col.Item().AlignCenter().Text(_model.Seller?.Subtext2 ?? string.Empty);
                        // col.Item().AlignCenter().Text(_model.Seller?.Subtext3 ?? string.Empty);
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
                column.Item().Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(3).Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Nombre y apellido: ").SemiBold();
                            text.Span(_model.Customer?.Name ?? string.Empty);
                        });
                        // col.Item().Text(text =>
                        // {
                        //     text.Span("Nº de documento: ").SemiBold();
                        //     text.Span(_model.Customer?.DocumentNumber ?? string.Empty);
                        // });
                        col.Item().Text(text =>
                        {
                            text.Span("Medio de pago: ").SemiBold();
                            text.Span("Billetera Digital Bipay");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Fecha de operación: ").SemiBold();
                            text.Span(_model.Details?.IssueDate.ToString("D"));
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Celular(Bipay): ").SemiBold();
                            text.Span("997 *** 563");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("ID transacción Bipay: ").SemiBold();
                            text.Span("235614");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Nº de comprobante: ").SemiBold();
                            text.Span("UNAJ-2026-0A54AEC8");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("ID solicitud UNAJ: ").SemiBold();
                            text.Span("0a54aec8-d872-8e8e-48f4-3a212ed15e23");
                        });

                        // Tipo de documento DNI
                    });
                    row.RelativeItem(2).Column(col =>
                    {
                        // col.Item().Text(text =>
                        // {
                        //     text.Span("Fecha de operación: ").SemiBold();
                        //     text.Span(_model.Details?.IssueDate.ToString("d", cultureInfo) ?? string.Empty);
                        // });
                    });
                });

                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                // Tabla de Items
                column.Item().Element(ComposeTable);

                // Sección de Totales
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


                // Firma / Pie de Página Informativo (QR e información de pago)
                // column.Item().Column(col =>
                // {
                //     if (!string.IsNullOrEmpty(_model.HashImagePath))
                //     {
                //         col.Item().Width(50).Image(_model.HashImagePath);
                //     }
                //     col.Item().Text(a =>
                //     {
                //         a.Span("Codigo hash: ").SemiBold();
                //         a.Span(_model.HashCode ?? string.Empty);
                //     });
                //     col.Item().Text(a =>
                //     {
                //         a.Span("Condicion pago: ").SemiBold();
                //         a.Span(_model.PaymentMethod ?? string.Empty);
                //     });
                // });

                column.Item().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
                {
                    // row.Spacing(50);

                    row.RelativeItem(10).Column(a =>
                    {
                        // a.Spacing(5);
                        a.Item().Text(text =>
                        {
                            // text.Span("Mensaje: ").SemiBold();
                            text.Span("AUTENTICIDAD DEL DOCUMENTO").FontSize(8);
                        });

                        a.Item().Text(text =>
                        {
                            // text.Span("Mensaje de advertencia: ").SemiBold();
                            text.Span(
                                    "Este comprobante fue generado automácamente tras confirmar el pago en Bipay.Para verificar su autencidad, ulice el código o el código QR.")
                                .FontSize(8);
                        });
//                         Código de verificación
// EA828F8E361C
// URL de verificaciónhttps://pagos.unaj.edu.pe/verificar?r=0a54aec8-
// d872-8e8e-48f4-3a212ed15e23&c=EA828F8E361C
                        a.Item().Text(text =>
                        {
                            text.Span("Código de verificación: ").FontSize(8);
                            text.Span("EA828F8E361C").FontSize(8);
                        });

                        a.Item().Text(text =>
                        {
                            text.Span("URL de verificación: ").FontSize(8);
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
                        // text.Span("Mensaje: ").SemiBold();
                        text.Span(_model.Message ?? string.Empty).Italic().FontSize(7);
                    });
                    a.Item().Text(text =>
                    {
                        // text.Span("Mensaje de advertencia: ").SemiBold();
                        text.Span(_model.MessageWarning ?? string.Empty).FontSize(7);
                    });
                });

                // add Emido electrónicamente por UNAJ — UNAJ-2026-0A54AEC8
                column.Item().PaddingVertical(5).Column(col =>
                {
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Emido electrónicamente por UNAJ — ").SemiBold();
                        text.Span("UNAJ-2026-0A54AEC8");
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
                    // columns.RelativeColumn();
                    // columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("Codigo").Style(headerStyle);
                    header.Cell().Text("Concepto").Style(headerStyle);
                    // header.Cell().AlignRight().Text("P.U.").Style(headerStyle);
                    // header.Cell().AlignRight().Text("C.").Style(headerStyle);
                    header.Cell().AlignRight().Text("Importe").Style(headerStyle);

                    header.Cell().ColumnSpan(3).PaddingTop(2).Border(0.5F).BorderColor(Colors.Grey.Darken1);
                });

                if (_model.Items != null)
                {
                    foreach (var item in _model.Items)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Index}");
                        table.Cell().Element(CellStyle).Text(item.Description ?? string.Empty);
                        // table.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice:C}");
                        // table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.Total:C}");

                        static IContainer CellStyle(IContainer container) => container.BorderBottom(0.5F)
                            .BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
                    }
                }
            });
        }
    }
}
