using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

QuestPDF.Settings.License = LicenseType.Community;
	    
// code in your main method
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.MarginLeft(2.5f, Unit.Centimetre);
        page.MarginRight(2.5f, Unit.Centimetre);
        
        page.MarginBottom(1.5f, Unit.Centimetre);
        page.MarginTop(1.5f, Unit.Centimetre);
        
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(9));
        

        page.Content()
            .Column(x =>
            {
                var styleTextCell = TextStyle.Default.FontSize(7);
                static IContainer CellNormal(IContainer container) => container.Border(1)
                    .BorderColor(Colors.Grey.Medium)
                    .PaddingVertical(10)
                    .PaddingHorizontal(4);

                static IContainer CellTitle(IContainer container) => container.Border(1)
                    .BorderColor(Colors.Grey.Medium)
                    .Padding(5);
                
                x
                    .Item()
                    .Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("ASISTENCIA DE COMISION EN EL EXAMEN DE ADMISION MODALIDAD CEPRE SEGUNDA FASE").Bold();
                    });

                x.Item().Padding(4);
                
                x.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(22);
                        columns.ConstantColumn(180);
                        columns.ConstantColumn(100);
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(100);
                    });

                    table.Cell().ColumnSpan(5).Element(CellTitle).Text(texttt =>
                    {
                        texttt.AlignCenter();
                        texttt.Span("DOMINGO 17 DE MARZO").Bold();
                    });
                    
                    table.Cell().Element(CellTitle).Text("N°");
                    table.Cell().Element(CellTitle).Text("APELLIDOS Y NOMBRES");
                    table.Cell().Element(CellTitle).Text("ROL");
                    table.Cell().Element(CellTitle).Text("DNI");
                    table.Cell().Element(CellTitle).Text("FIRMA");

                    var counter = 1;
                    for (int i = 0; i < 20; i++)
                    {
                        table.Cell().Element(CellNormal).Text(a =>
                        {
                            a.AlignCenter();
                            a.Span((counter++).ToString());
                        });
                        table.Cell().Element(CellNormal).Text("Washington Acero Mamani Mamani".ToUpperInvariant()).Style(styleTextCell);
                        table.Cell().Element(CellNormal).Text(string.Empty).Style(styleTextCell);
                        table.Cell().Element(CellNormal).Text("71449257").Style(styleTextCell);
                        table.Cell().Element(CellNormal).Text(string.Empty).Style(styleTextCell);
                    }
                });
            });
    });
})
.ShowInPreviewer();