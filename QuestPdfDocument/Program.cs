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

Document.Create(container =>
{
    container.Page(page =>
    {
        page.ConfigureBasePage();
        // page.ApplyWatermark(opc =>
        // {
        //     opc.Enabled = true;
        //     opc.Text = "DOCUMENTO OFICIAL";
        //     opc.FontSize = 55;
        //     opc.Color = Colors.Grey.Lighten4;
        // });
        // TODO: customize
        page.BuildHeader(universidad);

        page.Content()
            .PaddingHorizontal(30)
            .Column(x =>
            {
                x.Spacing(5);

                x.Item().Text(opciones.FechaGenerada).AlignEnd().FontSize(10);
                x.Item().PaddingBottom(10)
                    .Text($"CARTA N° {opciones.SequenceStart}{opciones.NumeroCarta}")
                    .AlignStart().Underline().Bold();

                x.Item().Text("Señor(a):").AlignStart();
                x.Item().Text(docente.FullName.ToUpper()).AlignStart().Bold();
                x.Item().Text(docente.GradoAcademico).AlignStart();
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
                    t.Span("\tPor medio del presente documento, la ");
                    t.Span(universidad.Unidad).Bold();
                    t.Span($" de la {universidad.Nombre}, se dirige a su distinguida persona para expresarle un cordial saludo y, ");
                    t.Span("a la vez, informarle que, en mérito a su destacada trayectoria docente y compromiso con la formación integral ");
                    t.Span("de los estudiantes de nuestra casa superior de estudios, ha sido designado(a) como ");
                    t.Span("TUTOR(A) ACADÉMICO(A)").Bold().Underline();
                    t.Span(" del Programa de Tutoría Universitaria correspondiente al ");
                    t.Span(periodo.Ciclo).Bold();
                    t.Span(".");
                });

                x.Item().Text(t =>
                {
                    t.Justify();
                    t.Span("\tLa tutoría universitaria constituye un pilar fundamental en el modelo educativo de nuestra institución, ");
                    t.Span("orientado a garantizar el acompañamiento personalizado, académico y socioemocional de los estudiantes durante ");
                    t.Span("su trayectoria formativa. En concordancia con el Reglamento de Tutoría Universitaria aprobado por Resolución ");
                    t.Span("Rectoral N° 0457-2022-UNJ/R, el desarrollo de esta función reviste carácter obligatorio para el personal docente ");
                    t.Span("en el marco de su dedicación y funciones sustantivas.");
                });

                x.Item().Text(t =>
                {
                    t.Justify();
                    t.Span("\tEn atención a lo expuesto, se le comunica los detalles correspondientes a su designación:");
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

                    static void RenderCeldaEtiqueta(TableDescriptor td, string txt)
                    {
                        td.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4)
                            .Padding(4).Text(txt).Bold().FontSize(9).AlignCenter();
                    }
                    static void RenderCeldaValor(TableDescriptor td, string txt)
                    {
                        td.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                            .Padding(4).Text(txt).FontSize(9);
                    }

                    RenderCeldaEtiqueta(tabla, "CÓDIGO"); RenderCeldaValor(tabla, tutoria.CodigoTutoria);
                    RenderCeldaEtiqueta(tabla, "PERÍODO"); RenderCeldaValor(tabla, periodo.Semestre);
                    RenderCeldaEtiqueta(tabla, "ESCUELA"); RenderCeldaValor(tabla, universidad.Escuela);
                    RenderCeldaEtiqueta(tabla, "MODALIDAD"); RenderCeldaValor(tabla, tutoria.Modalidad.ToString());
                    RenderCeldaEtiqueta(tabla, "HORARIO"); RenderCeldaValor(tabla, tutoria.Horario);
                    RenderCeldaEtiqueta(tabla, "LOCAL"); RenderCeldaValor(tabla, tutoria.Local);
                    RenderCeldaEtiqueta(tabla, "ESTUDIANTES"); RenderCeldaValor(tabla, $"{tutoria.CantidadEstudiantesAsignados} estudiantes asignados");
                    RenderCeldaEtiqueta(tabla, "SESIONES MÍNIMAS"); RenderCeldaValor(tabla, $"{periodo.CantidadSesionesRequeridas} sesiones");
                    RenderCeldaEtiqueta(tabla, "INICIO TUTORÍA"); RenderCeldaValor(tabla, periodo.FechaInicioTutorias.ToString("dd 'de' MMMM 'de' yyyy"));
                    RenderCeldaEtiqueta(tabla, "TÉRMINO TUTORÍA"); RenderCeldaValor(tabla, periodo.FechaFinTutorias.ToString("dd 'de' MMMM 'de' yyyy"));
                });

                x.Item().PaddingBottom(5).Text("I. FUNCIONES Y COMPROMISOS DEL TUTOR ACADÉMICO:").Bold().FontSize(10).Underline();
                x.Item().Column(c1 =>
                {
                    foreach (var (c, i) in compromisos.Select((co, inx) => (co, inx + 1)))
                        c1.Item().PaddingLeft(5).PaddingVertical(2).Text(tx =>
                        {
                            tx.Justify();
                            tx.Span($"{i}. ").Bold();
                            tx.Span(c);
                        });
                });

                x.Item().PaddingTop(10).PaddingBottom(5).Text("II. EJES TEMÁTICOS DEL PROGRAMA DE TUTORÍA:").Bold().FontSize(10).Underline();
                x.Item().Column(c2 =>
                {
                    foreach (var (e, i) in tutoria.EjesTematicos.Select((ej, inx) => (ej, inx + 1)))
                        c2.Item().PaddingLeft(5).PaddingVertical(1).Text(tx =>
                        {
                            tx.Span($"{i}. ").Bold().FontSize(9);
                            tx.Span(e).FontSize(9);
                        });
                });

                x.Item().PageBreak();

                x.Item().PaddingTop(5).PaddingBottom(5).Text("III. RELACIÓN DE ESTUDIANTES ASIGNADOS:").Bold().FontSize(10).Underline();

                x.Item().BuildTable(estudiantes, b =>
                {
                    b.Column(e => e.CodigoUniversitario, "CÓDIGO", o => o
                        .FixedWidth(2.5f, Unit.Centimetre)
                        .Aligned(HorizontalAlignment.Center));

                    b.Column(e => e.FullName, "APELLIDOS Y NOMBRES", o => o
                        .RelativeWidth(2));

                    b.Column(e => e.SemestreAcademico, "SEM.", o => o
                        .FixedWidth(1.5f, Unit.Centimetre)
                        .Aligned(HorizontalAlignment.Center));

                    b.Column(e => e.ModalidadIngreso, "MOD. INGRESO", o => o
                        .FixedWidth(1.5f, Unit.Centimetre)
                        .Aligned(HorizontalAlignment.Center)
                        .FontSize(7));

                    b.Column(e => e.CorreoInstitucional, "CORREO INSTITUCIONAL", o => o
                        .RelativeWidth(1)
                        .FontSize(7)
                        .AsLink());

                    b.Column(e => e.PromedioPonderado, "PROM. PON.", o => o
                        .FixedWidth(1.5f, Unit.Centimetre)
                        .Aligned(HorizontalAlignment.Center)
                        .Bold()
                        .Format(v => Convert.ToDecimal(v).ToString("0.00"))
                        .TextColor((est, _) => est.PromedioPonderado < 12
                            ? Colors.Red.Medium
                            : est.PromedioPonderado < 14
                                ? Colors.Orange.Medium
                                : Colors.Green.Medium));
                },
                opc =>
                {
                    opc.ShowHeader = true;
                    opc.ShowRowNumbers = true;
                    opc.RowNumberColumnWidth = 0.8f;
                },
                totalRows: tutoria.CantidadEstudiantesAsignados);

                x.Item().PaddingTop(15).Text(t =>
                {
                    t.Justify();
                    t.Span("\tCabe indicar que el incumplimiento injustificado de las funciones y compromisos detallados en la presente ");
                    t.Span("designación será evaluado conforme a las normas estatutarias y reglamentarias de la Institución, pudiendo ");
                    t.Span("derivar en la activación de los procedimientos administrativos correspondientes. Por otro lado, el desempeño ");
                    t.Span("destacado en esta función será tomado en cuenta para los procesos de evaluación docente y reconocimientos ");
                    t.Span("institucionales.");
                });

                x.Item().PaddingVertical(10).Text(t =>
                {
                    t.Justify();
                    t.Span("Sin otro particular y agradeciendo de antemano su compromiso con la misión formativa de nuestra universidad, ");
                    t.Span("me dirijo a usted para alcanzar el primer contacto con sus estudiantes tutorados el día ");
                    t.Span(periodo.FechaInicioTutorias.AddDays(2).ToString("dd 'de' MMMM 'de' yyyy")).Bold();
                    t.Span(" en el horario y local establecidos, para el desarrollo de la sesión de presentación y diagnóstico grupal. ");
                    t.Span("Se solicita acuse de recibido de la presente carta mediante el sistema académico institucional dentro de ");
                    t.Span("los tres (03) días hábiles siguientes a su recepción.");
                });

                x.Item().PaddingBottom(5).Text(t =>
                {
                    t.Span("Para cualquier consulta o coordinación, podrá comunicarse con la Unidad de Tutoría a través del correo ");
                    t.Span("tutoria.bienestar@unj.edu.pe").Italic().Underline();
                    t.Span(" o al teléfono anexo 128.");
                });

                x.Item().PaddingVertical(10).Text("Atentamente,").AlignCenter();

                x.Item().PaddingTop(20).Column(cf =>
                {
                    cf.Item().AlignCenter().Text("_______________________________").FontSize(10);
                    cf.Item().AlignCenter().Text(universidad.JefeUnidadTutoria).Bold().FontSize(10);
                    cf.Item().AlignCenter().Text($"Jefe de la {universidad.Unidad}").FontSize(9);
                    cf.Item().AlignCenter().Text(universidad.Nombre).FontSize(9);
                });

                x.Item().PaddingTop(20).Text("Documento generado por el Sistema de Gestión de Tutoría Universitaria - AKDEMIC TUTORÍA")
                    .FontSize(8).Italic().FontColor(Colors.Grey.Lighten1).AlignCenter();
            });
        
        // TODO: customize footer
        page.BuildFooter(universidad, footerMessage: null);
    });
})
// .GeneratePdf("fole.pdf")
.ShowInCompanion()
;
