using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace CertificadoTutoriaQuestPdf;

public class Program
{
    public static void Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var model = new CertificadoTutoriaModel();

        Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x =>
                        x.FontSize(11).FontColor(InstitutionColors.ColorGrisOscuro).FontFamily("Calibri"));

                    page.Content().Border(3).BorderColor(InstitutionColors.ColorDorado)
                        .Padding(1, Unit.Centimetre).Column(col =>
                        {
                            col.Spacing(8);

                            // Encabezado
                            col.Item().AlignCenter().Text(model.Universidad).SemiBold().FontSize(18)
                                .FontColor(InstitutionColors.ColorAzulPrincipal);
                            col.Item().AlignCenter().Text(model.EscuelaProfesional).SemiBold().FontSize(11)
                                .FontColor(InstitutionColors.ColorAzulMedio);
                            col.Item().AlignCenter().Text(model.TipoTutoria).SemiBold().FontSize(11)
                                .FontColor(InstitutionColors.ColorAzulMedio);

                            col.Item().PaddingTop(10).AlignCenter().Text(model.AnnoNombre).FontSize(8).Italic()
                                .FontColor("#666666");

                            // Línea decorativa
                            col.Item().PaddingVertical(5).LineHorizontal(2)
                                .LineColor(InstitutionColors.ColorDorado);

                            // Título de la Constancia
                            col.Item().PaddingTop(15).AlignCenter().Text(model.CodigoConstancia).Bold()
                                .FontSize(20).FontColor(InstitutionColors.ColorAzulPrincipal);

                            // Cuerpo
                            col.Item().PaddingTop(15).AlignCenter().Text(text =>
                            {
                                text.Span(
                                    "El que suscribe, Docente Tutor o Coordinador de Tutoría de la Escuela Profesional de ");
                                text.Span(model.EscuelaProfesional).SemiBold()
                                    .FontColor(InstitutionColors.ColorAzulPrincipal);
                                text.Span(" de la ");
                                text.Span(model.Universidad).SemiBold()
                                    .FontColor(InstitutionColors.ColorAzulPrincipal);
                                text.Span(".");
                            });

                            col.Item().PaddingTop(12).AlignCenter().Text("HACE CONSTAR QUE:").Bold().FontSize(24)
                                .FontColor(InstitutionColors.ColorAzulPrincipal);

                            col.Item().PaddingTop(12).AlignCenter().Text(text =>
                            {
                                text.Span("El Estudiante ");
                                text.Span(model.NombreEstudiante).SemiBold().FontSize(12)
                                    .FontColor(InstitutionColors.ColorAzulPrincipal);
                                text.Span(
                                    " ha cumplido con las actividades programadas de tutoría en el semestre académico ");
                                text.Span(model.SemestreAcademico).SemiBold()
                                    .FontColor(InstitutionColors.ColorAzulPrincipal);
                                text.Span(
                                    ", en concordancia al Reglamento y Normatividad del \"Sistema de Tutoría Universitaria\", (aprobado según Resolución ");
                                text.Span(model.Resolucion).SemiBold()
                                    .FontColor(InstitutionColors.ColorAzulPrincipal);
                                text.Span(").");
                            });

                            col.Item().PaddingTop(8).AlignCenter()
                                .Text(
                                    "Se expide la presente constancia a solicitud del interesado para los fines pertinentes.")
                                .FontSize(9).Italic();

                            // Fecha y Firma
                            col.Item().PaddingTop(15).AlignCenter().Text(model.Fecha).FontSize(10);

                            col.Item().PaddingTop(40).AlignCenter().Column(firma =>
                            {
                                firma.Item().Width(200).BorderTop(1)
                                    .BorderColor(InstitutionColors.ColorGrisOscuro).PaddingTop(5).AlignCenter()
                                    .Text(model.NombreFirmante)
                                    .SemiBold().FontColor(InstitutionColors.ColorAzulPrincipal);
                                firma.Item().AlignCenter().Text("TUTOR").FontSize(8).FontColor("#666666");
                            });
                        });
                });
            })
            .ShowInPreviewer();

        Console.WriteLine("PDF generado exitosamente: CertificadoTutoria.pdf");
    }
}

public class InstitutionColors
{
    public const string ColorAzulPrincipal = "#003366";
    public const string ColorAzulMedio = "#005B99";
    public const string ColorAzulClaro = "#4DA3FF";
    public const string ColorDorado = "#FFCC33";
    public const string ColorGrisOscuro = "#333333";
    public const string ColorGrisClaro = "#F5F5F5";
}

// TODO: translate to english variables
public class CertificadoTutoriaModel
{
    public string Universidad { get; set; } = "UNIVERSIDAD NACIONAL DE JULIACA";
    public string EscuelaProfesional { get; set; } = "ESCUELA PROFESIONAL DE INGENIERIA TEXTIL Y DE CONFECCIONES";
    public string TipoTutoria { get; set; } = "TUTORIA UNIVERSITARIA";
    public string AnnoNombre { get; set; } = "\"Año de la lucha contra la corrupción e impunidad\"";
    public string CodigoConstancia { get; set; } = "CONSTANCIA DE TUTORIA 8601-2026-1-UNAJ";
    public string NombreEstudiante { get; set; } = "IDME MAMANI MARIENNY";
    public string SemestreAcademico { get; set; } = "2026-1";
    public string Resolucion { get; set; } = "N° 1415-2015-R-UNAJ";
    public string Fecha { get; set; } = "Juliaca C.U. Abril del 2026";
    public string NombreFirmante { get; set; } = "HUANATICO SUAREZ ELIZABETH";
}
