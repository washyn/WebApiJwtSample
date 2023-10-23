using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.ExampleInvoice
{
    class InvoiceDocument : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(0.5f, Unit.Centimetre);
                page.MarginHorizontal(2f, Unit.Centimetre);
                
                page.PageColor(Colors.White);
                page.DefaultTextStyle(style => style.FontSize(10));
                
                page.Header()
                    .Column(col =>
                    {
                        col.Item().Text("Departamento de inteligencia.").FontSize(9);
                        col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Red.Medium);
                    });


                // for simplicity, you can also use extension method described in the "Extending DSL" section
                static IContainer Block(IContainer container)
                {
                    return container
                        .Border(1)
                        .BorderColor(Colors.Grey.Medium)
                        //.Background(Colors.Grey.Medium)
                        //.ShowOnce()
                        //.MinWidth(50)
                        //.MinHeight(50)
                        //.AlignCenter()
                        //.AlignMiddle()
                        ;
                }


                page.Content()
                    .PaddingVertical(0.2f, Unit.Centimetre)
                    .Column(x =>
                    {
                        //
                        //
                        //
                        x.Spacing(20);
                        
                        x.Item().Text("aaaaaaaaaaaaa");
                        x.Item().Text("Chester cahnged ddd");

                        x.Item()
                        .MinimalBox()
                        //.Padding(10)
                        //.Border(1)
                        .Table(table =>
                        {
                            
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Cell().ColumnSpan(4).Text("Total width: 300px");
                            table.Cell().Element(Block).Text("50px...");
                            table.Cell().Element(Block).Text("100px");
                            table.Cell().Element(Block).Text("100px");
                            table.Cell().Element(Block).Text("150px");
                        });

                        // ver filas,,,

                        x.Item()
                        //.Padding(2)
                        //.BorderColor(Colors.Red.Medium)
                        .Table(t => 
                        {
                            t.ColumnsDefinition(col => {
                                col.RelativeColumn(1);
                                col.RelativeColumn(1);
                            });

                            //t.Cell().Element(Block).Text("50px... ......");
                            //// dividir este elemento en 2... arriba pequeno con el nombre y abajo grande con el espacio en blanco...
                            //t.Cell().Element(Block).Text("100px");
                            //t.Cell().Element(Block).Text("100px");
                            for (int i = 0; i < 7; i++)
                            {
                                t.Cell().Element(Block).Column(item =>
                                {
                                    item.Item().AlignCenter().Text("dddd");
                                    item.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                                    item.Item().Padding(20);
                                    item.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                                });
                            }
                        });


                        //x.Item()
                        //.Row(row => 
                        //{
                        //    row.RelativeItem(1).Column(item =>
                        //    {
                        //        item.Item().AlignCenter().Text("dddd");
                        //        item.Item().LineHorizontal(1).LineColor(Colors.Green.Medium);
                        //        // separator
                        //        item.Item().Padding(20);
                        //    });
                        //});

                    });
                
                page.Footer()
                    .Column(a =>
                    {
                        a.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        a.Item().AlignRight().Text( x =>
                        {
                            x.Span("wiiiiiiiii");
                            x.Span("Pagina ").FontSize(9);
                            x.CurrentPageNumber().FontSize(9);
                            x.Span(" de ").FontSize(9);
                            x.TotalPages().FontSize(9);
                        });
                    });
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
            var document = new InvoiceDocument();

            // Generate PDF file and show it in the default viewer
            // document.GeneratePdfAndShow();
            
            // Or open the QuestPDF Previewer and experiment with the document's design
            // in real-time without recompilation after each code change
            // https://www.questpdf.com/document-previewer.html
            document.ShowInPreviewer();
        }
    }
}