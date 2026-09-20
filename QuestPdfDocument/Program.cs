using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfDocument;

QuestPDF.Settings.License = LicenseType.Community;



var document = Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.MarginHorizontal(1, Unit.Centimetre);
        page.MarginVertical(0.75F, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontFamily("Calibri"));
        page.DefaultTextStyle(x => x.FontSize(11));

        if (true)
        {
            page.Background()
                .AlignMiddle()
                .AlignCenter()
                .Rotate(-45f)
                .Text("CONFIDENCIAL")
                .Bold()
                .FontSize(60)
                .FontColor(Colors.Red.Lighten4)
                ;
        }

        // add as extension method
        page.Header()
            .PaddingHorizontal(30)
            .Column(x =>
            {
                x.Item().Row(row =>
                {
                    // 12 col
                    row.Spacing(10);
                    row.RelativeItem(3)
                        .Width(80)
                        .Image(Convert.FromBase64String(Data.File));
                    row.RelativeItem(10)
                        .DefaultTextStyle(a => a.FontSize(9))
                        .Column(a =>
                        {
                            a.Item().Text("Universidad nacional de juliaca").AlignCenter().FontColor(Colors.Grey.Darken1);
                            a.Item().Text("Unidad de tutoria - Generado AKDEMIC.TUTORIA").AlignCenter().FontColor(Colors.Grey.Darken1);
                            a.Item().Text("Año de la Esperanza y el Fortalecimiento de la Democracia").AlignCenter().FontColor(Colors.Grey.Darken1);
                        });
                    row.RelativeItem(3)
                        .Width(80)
                        .Image(Convert.FromBase64String(Data.File));
                });

                x.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);
            });

        page.Content()
            .PaddingHorizontal(30)
            .Column(x =>
            {
                x.Spacing(5);
                // x.Item()
                //     .Text("Año de la Esperanza y el Fortalecimiento de la Democracia")
                //     .FontSize(10)
                //     .AlignCenter()
                //     .Italic()
                //     .FontColor(Colors.Grey.Lighten1);

                // x.Item().PaddingVertical(10).Text($"{"_documentOptions.FechaGenerada"}").AlignEnd();

                x.Item().PaddingBottom(10)
                    .Text($"CARTA Nº {"_documentOptions.SequenceStart"}{"_documentOptions.NumeroCarta"}").AlignStart()
                    .Underline().Bold();

                // x.Item().Text($"{MapGender(docente.Genero)}:").AlignStart();
                x.Item().Text($"{"docente.FullName"}").AlignStart().Bold();
                x.Item().PaddingBottom(10).Text($"PRESENTE.-").AlignStart().Underline();


                // var asuntoText = string.Format(_documentOptions.Asunto, docente.Comision);
                //INVITACIÓN A PARTICIPAR EN LA COMISIÓN DE ELABORACIÓN DEL
                //  EXAMEN DE ADMISIÓN CEPRE 2024-II.

                if (string.Equals("JURADO DE AULA", "", StringComparison.OrdinalIgnoreCase))
                {
                    //   t.Span(docente.RolName.ToUpper()).Bold();
                    x.Item().Text(
                            $"ASUNTO:  INVITACIÓN A PARTICIPAR COMO JURADO DE AULA EN EL EXAMEN DE ADMISIÓN MODALIDAD CEPRE 2024-II.")
                        .Bold();
                }
                else
                {
                    // t.Span("COMISIÓN DE ELABORACIÓN".ToUpper()).Bold();
                    x.Item().Text(
                            $"ASUNTO:  INVITACIÓN A PARTICIPAR EN LA COMISIÓN DE ELABORACIÓN DEL EXAMEN DE ADMISIÓN MODALIDAD CEPRE 2024-II.")
                        .Bold();
                }


                //x.Item().Text($"ASUNTO:  {asuntoText}").Bold();
                var text = @"De mi especial consideración;";

                x.Item().PaddingVertical(5).Text(text);
                x.Item().Text(t =>
                {
                    t.Justify();

                    t.Span(
                        "\t Por medio del presesente documento me dirijo a su distinguida persona para expresarle un cordial saludo, asimismo informarle que este ");
                    t.Span("").Bold().Underline();
                    t.Span(" se desarrollará el examen de admisión en su modalidad ");
                    t.Span("");
                    t.Span(".");

                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                    t.Span(Placeholders.LoremIpsum());
                });

                x.Item().Text(t =>
                {
                    t.Justify();

                    if (string.Equals("JURADO DE AULA", "", StringComparison.OrdinalIgnoreCase))
                    {
                        t.Span("Por lo anterior, esta dirección le invita a participar como ");
                        t.Span("").Bold();
                    }
                    else
                    {
                        t.Span("Por lo anterior, esta Dirección le invita a participar en la ");
                        t.Span("COMISIÓN DE ELABORACIÓN".ToUpper()).Bold();
                    }

                    //t.Span(" en calidad de ....");
                    if (string.Equals("JURADO DE AULA", "", StringComparison.OrdinalIgnoreCase))
                    {
                        t.Span(
                            " en el proceso en mención; asismismo, poner de su conocimiento que deberá realizar coordinaciones con el responsable de ejecucion de examen.");
                        t.Span(
                            "Asimismo debera suscribirse a las declaraciones juradas de:Declaracion jurada de no tener vinculación con academias preuniversitarias,");
                        // lista de items
                        // agregar 3 puntos ...
                    }
                    else
                    {
                        t.Span(
                            " en el proceso en mención; asismismo, poner de su conocimiento que deberá realizar coordinaciones con el responsable de elaboracion de examen.");
                    }
                });

                x.Item().PaddingBottom(10).Text(t =>
                {
                    t.Justify();
                    t.Span("despedida");
                });

                x.Item().Text($"Atentamente,").AlignCenter();
            });

        // add as extension method
        page.Footer()
            .Column(x =>
            {
                // x.Item().Text("C.C.: Archivo");
                x.Item().PaddingVertical(5)
                    .PaddingHorizontal(30).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);
                x
                    .Item()
                    .AlignCenter()
                    .Text(
                        "Av. Nueva Zelandia N\u00b0 631 Urbanización la capilla Teléfono 328722-Juliaca - Puno - Perú")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);
                // page.Footer()
                //     .PaddingTop(25)
                //     .AlignCenter()
                //     .Text(text =>
                //     {
                //         text.CurrentPageNumber();
                //         text.Span(" / ");
                //         text.TotalPages();
                //     });
                x.Item()
                    .AlignRight()
                    .Text(a =>
                    {
                        a.CurrentPageNumber().FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                        a.Span("/").FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                        a.TotalPages().FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                    });
            });
    });
});

document.ShowInCompanion();
// add page nuber
// chechk example of extension methods



