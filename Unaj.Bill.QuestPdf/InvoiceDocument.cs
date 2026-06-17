using System;
using System.Globalization;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Invoice
{
    public class InvoiceDocument : IDocument
    {
        private readonly InvoiceDocumentViewModel _viewModel;

        public InvoiceDocument(InvoiceModel model, InvoiceTextResources textResources = null,
            CultureInfo cultureInfo = null)
        {
            _viewModel = InvoiceDocumentViewModelFactory.Create(
                model ?? throw new ArgumentNullException(nameof(model)),
                textResources,
                cultureInfo);
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
            container.Column(column =>
            {
                // Encabezado (Logo, Info Empresa, RUC / Nro Comprobante)
                column.Item().Row(row =>
                {
                    row.Spacing(10);

                    if (!string.IsNullOrEmpty(_viewModel.SellerLogoPath))
                    {
                        row.RelativeItem(3).Column(a => a.Item()
                            .Width(120).Image(_viewModel.SellerLogoPath));
                    }
                    else
                    {
                        row.RelativeItem().Column(a => a.Item()
                            .Text(string.Empty));
                    }

                    row.RelativeItem(5).Column(col =>
                    {
                        col.Item().AlignCenter().Text(_viewModel.SellerName).Medium().FontSize(12);
                        col.Item().AlignCenter().Text(_viewModel.SellerSubtext1);
                        col.Item().AlignCenter().Text(_viewModel.SellerSubtext2);
                    });

                    // row.RelativeItem().Column(a => a.Item()
                    //     .Text(string.Empty));

                    row.RelativeItem(3).AlignMiddle().Row(r =>
                    {
                        r.RelativeItem()
                            .Border(0.5F)
                            .BorderColor(Colors.Grey.Darken1)
                            .PaddingVertical(5)
                            .Column(col =>
                            {
                                col.Spacing(3);
                                col.Item().AlignCenter().Text(_viewModel.DocumentType);
                                // col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                                col.Item().AlignCenter().Text(_viewModel.DocumentNumber);
                            });
                    });
                });

                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);

                // Información del Cliente
                column.Item().Column(col =>
                {
                    foreach (var line in _viewModel.CustomerInfoLines)
                    {
                        col.Item().Text(text =>
                        {
                            text.Span(line.Label).SemiBold();
                            text.Span(line.Value);
                        });
                    }
                });


                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);


                column.Item().Element(ComposeTable);


                column.Item().PaddingBottom(5).Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);
                        col.Item().Text(_viewModel.AmountInWordsLine).Medium();
                    });
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text(_viewModel.TotalAmountLine)
                            .SemiBold();
                    });
                });


                column.Item().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
                {
                    row.RelativeItem(10).Column(a =>
                    {
                        a.Item().Text(text =>
                        {
                            text.Span(_viewModel.AuthenticityTitle).FontSize(8);
                        });
                        a.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten5);
                        a.Item().Text(text =>
                        {
                            text.Span(_viewModel.AuthenticityMessage).FontSize(8);
                        });

                        a.Item().Text(_viewModel.VerificationCodeLine).FontSize(8);
                        a.Item().Text(_viewModel.VerificationUrlLine).FontSize(8);
                    });
                    row.RelativeItem(2)
                        .AlignRight()
                        .Column(a =>
                        {
                            a.Item()
                                .Width(60)
                                .Height(60)
                                .Image(_viewModel.QrCode);
                        });
                });
                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.White);
                // can be addd padding top
                column.Item().Column(a =>
                {
                    a.Spacing(5);
                    a.Item().Text(text =>
                    {
                        text.Justify();
                        text.Span(_viewModel.Message).FontSize(7);
                    });
                    a.Item().Text(text =>
                    {
                        text.Span(_viewModel.MessageWarning).Italic().FontSize(7);
                    });
                });


                column.Item().PaddingVertical(5).Column(col =>
                {
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span(_viewModel.FooterPrefix)
                            .FontFamily("Calibri")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);

                        text.Span(_viewModel.FooterDocumentNumber)
                            .FontFamily("Calibri")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);
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
                    header.Cell().Text(_viewModel.TableHeaderCode).Style(headerStyle);
                    header.Cell().Text(_viewModel.TableHeaderConcept).Style(headerStyle);

                    header.Cell().AlignRight().Text(_viewModel.TableHeaderAmount).Style(headerStyle);

                    header.Cell().ColumnSpan(3).PaddingTop(2).Border(0.5F).BorderColor(Colors.Grey.Darken1);
                });

                if (_viewModel.Items != null)
                {
                    foreach (var item in _viewModel.Items)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Index);
                        table.Cell().Element(CellStyle).Text(item.Description);
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Total);

                        static IContainer CellStyle(IContainer container) => container.BorderBottom(0.5F)
                            .BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
                    }
                }
            });
        }
    }
}
