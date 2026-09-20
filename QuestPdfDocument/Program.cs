using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QuestPdfDocument;

QuestPDF.Settings.License = LicenseType.Community;

var universidad = MockData.Universidad;
var docente = MockData.Docente;
var periodo = MockData.Periodo;
var tutoria = MockData.Tutoria;
var opciones = MockData.OpcionesDocumento;
var estudiantes = MockData.ObtenerEstudiantesAsignados();
var compromisos = MockData.ObtenerCompromisosTutor();

var documento = Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.MarginHorizontal(1, Unit.Centimetre);
        page.MarginVertical(0.75F, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontFamily("Calibri"));
        page.DefaultTextStyle(x => x.FontSize(11));

        page.Header()
            .PaddingHorizontal(30)
            .Column(x =>
            {
                x.Item().Row(row =>
                {
                    row.Spacing(10);
                    row.RelativeItem(3)
                        .Width(80)
                        .Image(Convert.FromBase64String(universidad.LogoBase64));
                    row.RelativeItem(10)
                        .DefaultTextStyle(a => a.FontSize(9))
                        .Column(a =>
                        {
                            a.Item().Text(universidad.Nombre.ToUpper()).AlignCenter().FontColor(Colors.Grey.Darken1).Bold();
                            a.Item().Text($"{universidad.Facultad}").AlignCenter().FontColor(Colors.Grey.Darken1);
                            a.Item().Text($"{universidad.Escuela}").AlignCenter().FontColor(Colors.Grey.Darken1);
                            a.Item().Text($"{universidad.Unidad}").AlignCenter().FontColor(Colors.Grey.Darken1).Italic();
                        });
                    row.RelativeItem(3)
                        .Width(80)
                        .Image(Convert.FromBase64String(universidad.LogoBase64));
                });

                x.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);
            });

        page.Content()
            .PaddingHorizontal(30)
            .Column(x =>
            {
                x.Spacing(5);

                x.Item().Text(opciones.FechaGenerada).AlignEnd().FontSize(10);

                x.Item().PaddingBottom(10)
                    .Text($"CARTA N° {opciones.SequenceStart}{opciones.NumeroCarta}")
                    .AlignStart()
                    .Underline().Bold();

                x.Item().Text($"Señor(a):").AlignStart();
                x.Item().Text(docente.FullName.ToUpper()).AlignStart().Bold();
                x.Item().Text($"{docente.GradoAcademico}").AlignStart();
                x.Item().Text($"Docente de la {universidad.Escuela}").AlignStart();
                x.Item().Text($"Código: {docente.CodigoDocente}").AlignStart();
                x.Item().PaddingBottom(10).Text("PRESENTE.-").AlignStart().Underline();

                x.Item().Text(t =>
                {
                    t.Span("ASUNTO: ").Bold();
                    t.Span("DESIGNACIÓN COMO TUTOR(A) ACADÉMICO(A) DEL PROGRAMA DE TUTORÍA UNIVERSITARIA - ");
                    t.Span(periodo.Semestre).Bold();
                });

                x.Item().PaddingVertical(5).Text("De mi especial consideración;");

                x.Item().Text(t =>
                {
                    t.Justify();
                    t.Span(
                        "\tPor medio del presente documento, la ");
                    t.Span(universidad.Unidad).Bold();
                    t.Span(
                        $" de la {universidad.Nombre}, se dirige a su distinguida persona para expresarle un cordial saludo y, " +
                        $"a la vez, informarle que, en mérito a su destacada trayectoria docente y compromiso con la formación integral " +
                        $"de los estudiantes de nuestra casa superior de estudios, ha sido designado(a) como ");
                    t.Span("TUTOR(A) ACADÉMICO(A)").Bold().Underline();
                    t.Span(
                        $" del Programa de Tutoría Universitaria correspondiente al ");
                    t.Span(periodo.Ciclo).Bold();
                    t.Span(".");
                });

                x.Item().Text(t =>
                {
                    t.Justify();
                    t.Span(
                        "\tLa tutoría universitaria constituye un pilar fundamental en el modelo educativo de nuestra institución, " +
                        $"orientado a garantizar el acompañamiento personalizado, académico y socioemocional de los estudiantes durante " +
                        $"su trayectoria formativa. En concordancia con el Reglamento de Tutoría Universitaria aprobado por Resolución " +
                        $"Rectoral N° 0457-2022-UNJ/R, el desarrollo de esta función reviste carácter obligatorio para el personal docente " +
                        $"en el marco de su dedicación y funciones sustantivas.");
                });

                x.Item().Text(t =>
                {
                    t.Justify();
                    t.Span(
                        "\tEn atención a lo expuesto, se le comunica los detalles correspondientes a su designación:");
                });

                x.Item().PaddingTop(5).PaddingBottom(10).Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.ConstantColumn(3.5f, Unit.Centimetre);
                        columnas.RelativeColumn(1);
                        columnas.ConstantColumn(3.5f, Unit.Centimetre);
                        columnas.RelativeColumn(1);
                    });

                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("CÓDIGO").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(tutoria.CodigoTutoria).FontSize(9);
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("PERÍODO").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(periodo.Semestre).FontSize(9);

                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("ESCUELA").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(universidad.Escuela).FontSize(9);
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("MODALIDAD").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(tutoria.Modalidad.ToString()).FontSize(9);

                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("HORARIO").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(tutoria.Horario).FontSize(9);
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("LOCAL").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(tutoria.Local).FontSize(9);

                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("ESTUDIANTES").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{tutoria.CantidadEstudiantesAsignados} estudiantes asignados").FontSize(9);
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("SESIONES MÍNIMAS").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{periodo.CantidadSesionesRequeridas} sesiones").FontSize(9);

                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("INICIO TUTORÍA").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(periodo.FechaInicioTutorias.ToString("dd 'de' MMMM 'de' yyyy")).FontSize(9);
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(4).Text("TÉRMINO TUTORÍA").Bold().FontSize(9).AlignCenter();
                    tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(periodo.FechaFinTutorias.ToString("dd 'de' MMMM 'de' yyyy")).FontSize(9);
                });

                x.Item().PaddingBottom(5).Text("I. FUNCIONES Y COMPROMISOS DEL TUTOR ACADÉMICO:").Bold().FontSize(10).Underline();

                x.Item().Column(columna =>
                {
                    foreach (var (compromiso, indice) in compromisos.Select((c, i) => (c, i + 1)))
                    {
                        columna.Item().PaddingLeft(5).PaddingVertical(2).Text(t =>
                        {
                            t.Justify();
                            t.Span($"{indice}. ").Bold();
                            t.Span(compromiso);
                        });
                    }
                });

                x.Item().PaddingTop(10).PaddingBottom(5).Text("II. EJES TEMÁTICOS DEL PROGRAMA DE TUTORÍA:").Bold().FontSize(10).Underline();

                x.Item().Column(columna =>
                {
                    foreach (var (eje, indice) in tutoria.EjesTematicos.Select((e, i) => (e, i + 1)))
                    {
                        columna.Item().PaddingLeft(5).PaddingVertical(1).Text(t =>
                        {
                            t.Span($"{indice}. ").Bold().FontSize(9);
                            t.Span(eje).FontSize(9);
                        });
                    }
                });

                x.Item().PageBreak();

                x.Item().PaddingTop(5).PaddingBottom(5).Text("III. RELACIÓN DE ESTUDIANTES ASIGNADOS:").Bold().FontSize(10).Underline();

                x.Item().Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.ConstantColumn(0.8f, Unit.Centimetre);
                        columnas.ConstantColumn(2.5f, Unit.Centimetre);
                        columnas.RelativeColumn(2);
                        columnas.ConstantColumn(1.5f, Unit.Centimetre);
                        columnas.ConstantColumn(1.5f, Unit.Centimetre);
                        columnas.RelativeColumn(1);
                        columnas.ConstantColumn(1.5f, Unit.Centimetre);
                    });

                    void CeldaCabecera(string texto)
                    {
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Darken2)
                            .Padding(3).AlignCenter().AlignMiddle()
                            .Text(texto).Bold().FontSize(8).FontColor(Colors.White);
                    }

                    CeldaCabecera("N°");
                    CeldaCabecera("CÓDIGO");
                    CeldaCabecera("APELLIDOS Y NOMBRES");
                    CeldaCabecera("SEM.");
                    CeldaCabecera("MOD. INGRESO");
                    CeldaCabecera("CORREO INSTITUCIONAL");
                    CeldaCabecera("PROM. PON.");

                    foreach (var (estudiante, indice) in estudiantes.Select((e, i) => (e, i + 1)))
                    {
                        var fondo = indice % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        var colorPromedio = estudiante.PromedioPonderado < 12 ? Colors.Red.Medium :
                                            estudiante.PromedioPonderado < 14 ? Colors.Orange.Medium :
                                            Colors.Green.Medium;

                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignCenter().Text($"{indice}").FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignCenter().Text(estudiante.CodigoUniversitario).FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignLeft().Text(estudiante.FullName).FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignCenter().Text(estudiante.SemestreAcademico).FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignCenter().Text(estudiante.ModalidadIngreso).FontSize(7);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignLeft().Text(estudiante.CorreoInstitucional).FontSize(7);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignCenter().Text(estudiante.PromedioPonderado.ToString("0.00")).FontSize(8).Bold().FontColor(colorPromedio);
                    }

                    for (var i = estudiantes.Count + 1; i <= tutoria.CantidadEstudiantesAsignados; i++)
                    {
                        var fondo = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).AlignCenter().Text($"{i}").FontSize(8).FontColor(Colors.Grey.Lighten1);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).Text("").FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).Text("").FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).Text("").FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).Text("").FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).Text("").FontSize(8);
                        tabla.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(fondo).Padding(2).Text("").FontSize(8);
                    }
                });

                x.Item().PaddingTop(15).Text(t =>
                {
                    t.Justify();
                    t.Span(
                        "\tCabe indicar que el incumplimiento injustificado de las funciones y compromisos detallados en la presente " +
                        "designación será evaluado conforme a las normas estatutarias y reglamentarias de la Institución, pudiendo " +
                        "derivar en la activación de los procedimientos administrativos correspondientes. Por otro lado, el desempeño " +
                        "destacado en esta función será tomado en cuenta para los procesos de evaluación docente y reconocimientos " +
                        "institucionales.");
                });

                x.Item().PaddingVertical(10).Text(t =>
                {
                    t.Justify();
                    t.Span(
                        "Sin otro particular y agradeciendo de antemano su compromiso con la misión formativa de nuestra universidad, " +
                        "me dirijo a usted para alcanzar el primer contacto con sus estudiantes tutorados el día ");
                    t.Span(periodo.FechaInicioTutorias.AddDays(2).ToString("dd 'de' MMMM 'de' yyyy")).Bold();
                    t.Span(
                        " en el horario y local establecidos, para el desarrollo de la sesión de presentación y diagnóstico grupal. " +
                        "Se solicita acuse de recibido de la presente carta mediante el sistema académico institucional dentro de " +
                        "los tres (03) días hábiles siguientes a su recepción.");
                });

                x.Item().PaddingBottom(5).Text(t =>
                {
                    t.Span(
                        "Para cualquier consulta o coordinación, podrá comunicarse con la Unidad de Tutoría a través del correo ");
                    t.Span("tutoria.bienestar@unj.edu.pe").Italic().Underline();
                    t.Span(" o al teléfono anexo 128.");
                });

                x.Item().PaddingVertical(10).Text("Atentamente,").AlignCenter();

                x.Item().PaddingTop(20).Column(colFirma =>
                {
                    colFirma.Item().AlignCenter().Text("_______________________________").FontSize(10);
                    colFirma.Item().AlignCenter().Text(universidad.JefeUnidadTutoria).Bold().FontSize(10);
                    colFirma.Item().AlignCenter().Text($"Jefe de la {universidad.Unidad}").FontSize(9);
                    colFirma.Item().AlignCenter().Text($"{universidad.Nombre}").FontSize(9);
                });

                x.Item().PaddingTop(20).Text("Documento generado por el Sistema de Gestión de Tutoría Universitaria - AKDEMIC TUTORÍA").FontSize(8).Italic().FontColor(Colors.Grey.Lighten1).AlignCenter();
            });

        page.Footer()
            .Column(x =>
            {
                x.Item().PaddingVertical(5)
                    .PaddingHorizontal(30).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);
                x
                    .Item()
                    .AlignCenter()
                    .Text(
                        $"{universidad.Direccion} Teléfono {universidad.Telefono} - {universidad.Ubicacion}")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);
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

documento.ShowInCompanion();
